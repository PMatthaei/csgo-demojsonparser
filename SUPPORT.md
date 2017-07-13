# Possible problems:
- As soon as you run DemoInfo Code, you get a message like this:

>"Additional information: The file or assembly
"DemoInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=dc7de83c756ec63d" or one of its dependencies have not been found ..."

-> Try building every project as 32-Bit(x64) 
 OR follow these guides:
 
 >This problem is related to Strong Name Validation. Open your AssemblyX in Ildasm.exe(C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin). Note its PublicKeyToken, lets say pkt123 for an example. Now open VS Command prompt in administrator mode and run the sn.exe command. Such as:

>sn -Vr *,pkt123
>Build your solution again and everything should be fine by now.

>But if not and you receive same error now also, then you need to run a different version of sn.exe. To locate that, go to Visual Studio >command prompt.

>c:\Program Files(x86)>dir /s sn.exe
>It may take 5-10 seconds and should give a list of sn.exe files. Go to the path and execute the sn.exe, required or belongs to you, as >shown above. If not sure which one to execute, execute all the sn.exe. That should and must solve your problem. If not, let me know and >let me carry forward the RnD again.

https://blogs.msdn.microsoft.com/kaevans/2008/06/18/getting-public-key-token-of-assembly-within-visual-studio/

- Your are experiencing Erros when deserializing the file/string:
-> Due the fact that i used inheritance to model similar classes, the parser from Json.NET needs information how to handle these classes.
So for now use JsonSerializerSettings to resolve this error. More info and possible fix soon.
