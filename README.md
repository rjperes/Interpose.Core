# Interpose.Core

## Introduction
Interpose.Core is a framework for doing dynamic interception of .NET code.
Interpose.Core targets .NET Core 2.0.

## Concepts
It supports the following interception mechanisms:

* Virtual method: only virtual methods of non-sealed classes can be intercepted
* Interface: any method declared in an interface can be intercepted
* Dynamic: any method or property can be implemented as long as the object can be treated as dynamic

These interception mechanisms all fall into one kind:

* Type interception: a new type is generated on the fly; examples are virtual method interception
* Instance interception: uses any existing instance; examples are interface and dynamic

## Usage
Instance interception (interface):

    var interceptor = new InterfaceInterceptor();
    var instance = new MyType();
    var handler = new MyHandler();
    var proxy = interceptor.Intercept(instance, typeof(IMyType), handler) as IMyType;

Type interception (virtual method):

    var interceptor = new VirtualMethodInterceptor();    
    var proxyType = interceptor.Intercept(typeof(MyType), typeof(MyHandler));

## Installation
You can either:

- Clone from GitHub: [http://github.com/rjperes/Interpose.Core](http://github.com/rjperes/Interpose.Core)
- Install via Nuget: [https://www.nuget.org/packages/Interpose.Core](https://www.nuget.org/packages/Interpose.Core).

## Contacts
If you see any value in this and wish to send me your comments, please do so through [GitHub](https://github.com/rjperes/Interpose.Core). Questions and suggestions are welcome too!

## Licenses
This software is distributed under the terms of the Free Software Foundation Lesser GNU Public License (LGPL), version 2.1 (see lgpl.txt).

## Copyright
You are free to use this as you wish, but I ask you to please send me a note about it.