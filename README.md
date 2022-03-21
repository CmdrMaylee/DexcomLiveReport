# DexcomLiveReport

A simple app that communicates with the Dexcom Share API, in order to retrieve one's most recent glucose values, fashioned as a terminal/console app by default.

The code in Program.cs acts as an example in how the program could work as a console application, and can be omitted if the code is repurposed.
Currently there is no way to adjust the login-credentials besides in the source-code before building the app. If I can stumble upon a way to do this as elegantly as possible(or if someone has suggestions, who are also willing to share), I'll put it in place.

The API has two different base-url's depending on locale, which boils down to either inside or outside of the US. By default this is set to use the non-us url, and as circumstances would have it, I have no clue if the US-url works equally as well.

As this is the first "real" project I was asked to do, feedback would be greatly appreciated.