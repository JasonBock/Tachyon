# Tachyon
A library to create proxies on the fly.

## Overview
Basically, this is a re-imaging of my [DynamicProxies](https://github.com/JasonBock/DynamicProxies) package. That uses `System.Reflection.Emit`, and frankly that's just not very friendly in the .NET Core world. Rather than try to get it and packages that I've created to support DynamicProxies (such as [EmitDebugging](https://github.com/JasonBock/EmitDebugging) and [AssemblyVerifier](https://github.com/JasonBock/AssemblyVerifier) to work correctly under .NET Core, I've decided to start fresh. This will use [Mono.Cecil](https://github.com/jbevain/cecil) to generate the IL. I'd like to add the IL file generation feature that I had with EmitDebugger, but that's not the primary goal.