This program makes use of this layout component: https://github.com/Xenira/LiveSplit-Websocket

It implements startorsplit, reset, unsplit, skip split and pause/resume via a global hotkey.

You need to add the websocket component to the layout and use control->Star Server (WS) in livesplit on the server.

Setup and logic should be easy in-software.

This requires .net 6 runtime to work properly.

//FFyte


Advanced:
There's a fallback in the source for blocking hotkeys, if you want to look at that.