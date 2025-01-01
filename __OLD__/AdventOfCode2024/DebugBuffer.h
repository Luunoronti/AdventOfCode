#pragma once

#include <iostream> 
#include <sstream>

namespace aoc
{
    // Custom stream buffer 
    class DebugBuffer : public std::stringbuf
    {
    public: DebugBuffer(bool enabled = true) : enabled(enabled) {}
          int sync() override
          {
              if(enabled)
              {
                  std::cout << str();
              }
              str("");
              return 0;
          }
          void setEnabled(bool enabled)
          {
              this->enabled = enabled;
          }
    private: bool enabled;
    };

    // Custom stream 
    class DebugStream : public std::ostream
    {
    public: DebugStream() : std::ostream(&buf), buf() {}
          void setEnabled(bool enabled)
          {
              buf.setEnabled(enabled);
          }
    private: DebugBuffer buf;
    };

    // Create a global instance of the custom stream 
    extern DebugStream dout;
}