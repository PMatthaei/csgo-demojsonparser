# Demojsonparser

Feed a valid* .dem and he will spit out a valid json containing information about every tick in every round of your game as well as some metadata about it.

This is a JSONParser using the following format : [Click here](https://github.com/PMatthaei/demojsonparser-csgo/blob/master/JSONFORMAT.md)

Data about the CS:GO demos is parsed from bytecode by:

DemoInfo - https://github.com/StatsHelix/demoinfo

This data is than serialized by Newtonsoft.JSON - http://www.newtonsoft.com/json


[*] Only GOTV replays newer than Juli 2015
