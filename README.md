IndentReplace
=============

I have the desire to develop open source projects. Some of them made ​​indentation spaces and it makes me angry. 

Example solutions described on this page
 http://stackoverflow.com/questions/2316677/can-git-automatically-switch-between-spaces-and-tabs
but this solution works under Linux only.

This tool allows you to automatically switch between spaces and tabs into git and represents the implementation expand/unexpand under windows (with .Net)

Example:
Need to add these lines to the file .git/config
 [filter "tabspace4"]
     smudge = c:/Tools/IndentReplace/IndentReplace.exe -s=4 -i=tab
     clean = c:/Tools/IndentReplace/IndentReplace.exe -s=4 -i=space

And this line to the file .git/info/attributes
 *.cs  filter=tabspace4

And run command:
 git checkout HEAD -- **