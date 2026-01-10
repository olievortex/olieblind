@echo off
cd /Users/oliev/source/repos/hop/olieblind/olieblind.purple
call C:\Users\oliev\miniconda3\Scripts\activate.bat
call conda activate uv_play
uv run %1 %2 %3 %4
