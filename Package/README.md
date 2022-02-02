# SassImporter for Unity 

## Installation

### Using the git repo

Add a git package in the Unity Package Manager with target:

https://github.com/arfinity-io/unity-sassimporter.git?path=/Package#v1.1.1

### Using openupm

Add package with openupm-cli

```
npx openupm-cli add io.arfinity.unity.packages.sassimporter
```

## Usage

To use this package simply add it in the Unity Package Manager. Scss files are then automatically imported and available as UIToolkit stylesheets.

Make sure you have a [Sass](https://sass-lang.com) compiler locally installed. E.g., for Dart Sass, you can install it as a standalone version on

- Windows with Chocolatey

	```
	choco install sass
	```

- MacOS with Homebrew

	```
	brew install sass/sass/sass
	```

You can also install a Javascript implementation of Sass using Node

```
npm install -g sass
```

though this is said to be less performant.


## Functionality

The importer reads SCSS files such that they can be referenced like USS files. Make sure that you only use USS-supported properties, units, and queries in the SCSS code.

SCSS files starting with an underscore are ignored.

## Optional Extras

If you don't want to install sass locally on any machine that builds/imports this project there is an optional package 'io.arfinity.unity.packages.sassexecutables' that ships the sass executables for Linux/MacOS/Windows.
