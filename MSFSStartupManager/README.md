# Notes

Pages:

  - intro; welcome, here's what the program does
  - initial scan
    - Your exe.xml is corrupt but bits of it _might_ work?
    - Your exe.xml is fine but you have no startup programs; you're done
    - Your exe.xml is fine and you have some startup programs. Here's what they appear to be.
    Please try running MSFS now. Does ANYTHING start up?
    If something starts up, go to contact page

  - try something else
    None of your programs are starting up.  If you have a little more time, can we try
    temporarily replacing your startup configuration with something we KNOW works?
    We'll make a backup of your configuration first, and show you where it is, just in
    case something goes wrong.

  - try something else 2
    ok, we've temporarily replaced your startup configuration. Please try MSFS again.
    Do you get a message saying "Startup programs ARE working"?
    We'll put your original configuration back when you've confirmed it's working or not (or exit this program).
    

  - contact
    - can we have your email?
    - if so, what can we do with it
  - preview
    - show report
    - submit button
  - completed
    - thank you!


    individual view models for pages?


MasterVM
  IPageVM : MovePrevious
  IPageVM : MoveNext
  PageVM1 : IPageVM
  PageVM2 : IPageVM

  OnExit - for exe.xml cleanup
