<img src="logo.jpg" width="192px"/>

Build Status
============

**Mono:** [![Build Status](https://travis-ci.org/sungiant/djinn.png?branch=development)](https://travis-ci.org/sungiant/djinn)

Getting Started
===============

To install Djinn, download the repository.  For the purposes of this guide lets assume you have cloned the repository to:

    ~/djinn/

Djinn run on mono, to build it you need the [Mono SDK](http://www.go-mono.com/mono-downloads/download.html) installed.

Now run:

    sh ~/djinn/scripts/install_djinn

This generates the file:

   ~/.djinn

which is responsible for defining where your djinn projects exist.  If you look at it:

    cat ~/.djinn

you can see that a default project has been set up to use the example specifications in your clone of the djinn repository:

    ~/djinn/example_specification/

Next edit:

    ~/djinn/scripts/install_djinn_aws_credentials

so that it contains the AWS credentials you want to use.

Now run:

    sh scripts/install_djinn_aws_credentials

This generates the file:

   ~/.djinn.aws

which contains your AWS access crendentials.

Now you should be able to run:

    djinn endpoints

to examine your machines.


Example: CPU Miner
------------------

To spin up then shutdown a machine first edit:

    ~/djinn/example_specification/blueprints/cpuminer.json

so that it references the login details for one of your scrypt mining workers.

Now to spin up the machine run:

    djinn provision example-zone cpuminer

Next we need to install the software:

    djinn configure example-zone cpuminer

Now we need to get things running:

    djinn deploy example-zone cpuminer

Now to shutdown:

    djinn destroy example-zone cpuminer


