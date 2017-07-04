# Possible problems:
- As soon as you run DemoInfo Code, you get a message like this:

>"Additional information: The file or assembly
"DemoInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dc7de83c756ec63d" or one of its dependencies have not been found ..."

-> Try building every project as 32-Bit(x64) 

- Your are experiencing Erros when deserializing the file/string:
-> Due the fact that i used inheritance to model similar classes, the parser from Json.NET needs information how to handle these classes.
So for now use JsonSerializerSettings to resolve this error. More info and possible fix soon.
