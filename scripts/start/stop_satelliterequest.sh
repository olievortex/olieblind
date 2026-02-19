#!/bin/sh
# Get the PID of the process
pid=$(pgrep -f "dotnet olieblind.cli.dll satelliterequest")

# Check if the process exists
if [ -n "$pid" ]; then
  # Terminate the process
  kill "$pid"
  echo "Process with PID $pid terminated."
else
  echo "Process 'olieblind.cli.dll satelliterequest' not found."
fi
