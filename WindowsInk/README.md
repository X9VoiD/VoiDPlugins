## WindowsInk

WindowsInk provides two new output modes

      Artist Mode (Windows Ink)
      Artist Mode (Relative Windows Ink)

This output mode enables pen pressure support for all apps compatible to Windows Ink API like:
  * Adobe Photoshop
  * Paint Tool SAI
  * Clip Studio Paint
  * Corel Painter
  * etc...

## Requirements

Install [VMulti](https://github.com/X9VoiD/vmulti-bin/releases/latest)

## Installation

Download WindowsInk plugin from [latest releases.](https://github.com/X9VoiD/VoiDPlugins/releases/latest)

Then copy zip contents to:

| Platform | Path |
| :-- | :-- |
| Windows | `%localappdata%\OpenTabletDriver\Plugins` |
| Linux | `~/.config/OpenTabletDriver/Plugins` |
| MacOS | `$HOME/Library/Application Support/OpenTabletDriver/Plugins` |

## Usage

Enable WindowsInk by selecting it as an output mode on the OutputModeSelector of Output tab.

## Acknowledgements

Huge thanks to [skyborgff](https://github.com/skyborgff) for the [base implementation](https://github.com/skyborgff/OpenTabletDriverPlugins/tree/WindowsInk)!