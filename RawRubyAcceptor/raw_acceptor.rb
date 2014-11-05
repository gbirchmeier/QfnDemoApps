require 'socket'


def checksumify msg
  ### sum all bytes up to but not including checksum field, then mod 256
  out = msg
  out = msg.sub(/10=[\d]*/,"") #remove any existing CheckSum
  out << "#{out.sum(8)}"
  out
end

def lengthify msg
  ### number of chars following BodyLength up to and including the delim before CheckSum
  out = msg
  out.sub!(/9=[\d]*/,"") #remove any existing BodyLength
  length = out.match(/(34=.*?)(?:10=[\d]*$|$)/)[1].length
  out.sub!(/35=/,"9=#{length}35=")
  out
end

def timify msg
  ### insert SendingTime after 34
  out = msg
  out.sub!(/52=[^]*/,"") # remove any preexisting SendingTime
  seqno = out.match(/34=([\d]*)/)[1]
  out.sub!(/34=[\d]*/,"34=#{seqno}52=#{Time.new.gmtime.strftime("%Y%m%d-%H:%M:%S")}")
end

# insert BodyLength/Checksum/SendingTime
def polish msg
  out = timify msg
  out = lengthify msg
  checksumify msg
end

def reflection(incoming)
  out = incoming
  out.sub!(/49=/,"x56=")
  out.sub!(/56=/,"49=")
  out.sub!(/x56=/,"56=")

  polish out
end


def read_fix_message(io)
  if(io.eof?)
    raise "Was disconnected, expected data"
  end

  m = ""
  # read to begining of MsgLen field
  m = io.gets("\0019=")
  # read contents of MsgLen field
  length = io.gets("\001")
  m += length
  length.chop!
  
  # regex checks to make sure length is an integer
  # if it isn't there is nothing we can do so
  # close the connection
  if( (/^\d*$/ === length) == nil )
    io.close
  end
  # read body
  m += io.read(Integer(length))
  # read CheckSum
  m += io.gets("\001")
  return m
end


PORT=5004
server = TCPServer.new("127.0.0.1",PORT)

loop {
  puts "Waiting for a connection."
  socket = server.accept
  puts "Connected."

  loop {
    msg_in = read_fix_message(socket)
    puts "RECEIVED: #{msg_in.gsub("","|")}"

    if msg_in.include?("35=A") || msg_in.include?("35=5")
      response = reflection(msg_in)
      puts "SENDING: #{response.gsub("","|")}"
      socket.write response
    end

    if msg_in.include?("35=5")
      sleep 2
      break
    end
  }

  break
}

