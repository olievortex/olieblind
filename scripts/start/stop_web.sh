#!/bin/sh
# Get the PID of the process
pid=$(pgrep -f "dotnet olieblind.web.dll")

# Check if the process exists
if [ -n "$pid" ]; then
  # Terminate the process
  kill "$pid"
  echo "Process with PID $pid terminated."
else
  echo "Process 'dotnet olieblind.web.dll' not found."
fi
