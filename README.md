# Demojsonparser

# How to set up
- Clone this project or download the zip file
- Add Json.Net with Nuget Package Manage to all subprojects depending on it
- Add [my forked project of DemoInfo](https://github.com/PMatthaei/demoinfo) to all subprojects depending on it
- Build and have fun...if you have problems preserve your fun and see [here](https://github.com/PMatthaei/demojsonparser-csgo/blob/master/SUPPORT.md)

# How to use
Feed a valid[*] .dem from a CS:GO match and the parser will spit out a valid json containing information about every tick in every round of your game as well as some metadata about it.

# What you get
This is a JSONParser using the following format : [Click here](https://github.com/PMatthaei/demojsonparser-csgo/blob/master/JSONFORMAT.md)

# Credit
Data from CS:GO demos (.dem) is parsed with a modified version of:
DemoInfo - https://github.com/StatsHelix/demoinfo 
This data is than serialized by Newtonsoft.JSON - http://www.newtonsoft.com/json


[*] Only GOTV replays newer than Juli 2015, see https://github.com/StatsHelix/demoinfo  
