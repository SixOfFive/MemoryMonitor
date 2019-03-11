# MemoryMonitor
Windows Memory Monitor and Process Killer

Run as the user on startup, will hide itself and run in the 
taskbar showing the users current ram count and process with 
the highest ram usage.

Default maximum ram usage is 4GB per process (if a process
goes over 4GB it will be killed within a minute).

Default check interval is 60 seconds.

These 2 defaults can be changed by using a settings file or
by changing the defaults and then recompiling.

Currently 64bit and compiled against .net framework 4.6.1 
but should be compatible with many versions of the .net 
framework and can be recompiled for 32 bit.
