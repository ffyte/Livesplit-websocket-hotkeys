This program makes use of this layout component: https://github.com/Xenira/LiveSplit-Websocket

It implements startorsplit, reset, unsplit, skip split and pause/resume via a global hotkey.

You need to add the websocket component to the layout and use control->Star Server (WS) in livesplit on the server.

Setup and logic should be easy in-software. It uses a (text) settings file to save your preferences.

This requires .net 6 runtime to work properly (for win7 compatibility).

//FFyte

Thanks to Ero & Cryze

Advanced:
There's a fallback in the source for blocking hotkeys, if you want to look at that.
Requires Websocket.Client nuget-package

Non-blocking hotkeys might work with modifiers (shift, alt, ctrl) 
