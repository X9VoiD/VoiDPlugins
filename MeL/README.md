# MeL

Machine Learning Plugin for OpenTabletDriver

## Parts

### [MeLFilter](https://github.com/X9VoiD/VoiDPlugins/wiki/MeLFilter)

### [MeLInterp](https://github.com/X9VoiD/VoiDPlugins/wiki/MeLInterp)

## Installation

Download MeL from [Latest Release.](https://github.com/X9VoiD/VoiDPlugins/releases/latest)

Then copy `MeL.dll` to:

| Platform | Path |
| :-- | :-- |
| Windows | `%localappdata%\OpenTabletDriver\Plugins` |
| Linux | `~/.config/OpenTabletDriver/Plugins` |
| MacOS | `$HOME/Library/Application Support/OpenTabletDriver/Plugins` |

## Configuration

These are the different configuration knobs for MeL.

### Offset (only in MeLFilter)

Amount of time in milliseconds to offset the prediction for the next point.

* Zero Offset - MeL will apply low latency cursor correction.
* Positive Offset - MeL will try to predict future cursor position.
* Negative Offset - MeL will delay cursor position to smooth out movement.

    High positive offset may result in inaccurate and erratic cursor movement.  
    High negative offset does not guarantee smoother cursor movement.

### Samples

Determines how long of a history to keep to feed into the filter.

A sample is defined as an update in cursor position.

* `Sample count` is generally a trade-off between CPU usage and accuracy.  

    Samples must always be higher than Complexity!

### Complexity

Determines the complexity of prediction to compute by the filter.

### Weight

Determines how much more important the later samples will be. This modifies how strict the MeL is with overshooting.

* A Weight 2 for example will say that the later samples will be two times more important.

    A high Weight like 1.5 might introduce jagged edges on the cursor.  
    A low Weight like 1.1 below will cause overshooting when using with low Complexity.

## Recommended Settings

| Settings | Min Recommended Value | Max Recommended Value | Default Value |
| :--- | :--- | :--- | :--- |
| Offset | -10 | 10 | **0** |
| Samples | 16 | 25 | **20** |
| Complexity | 1 | 3 | **2** |
| Weight | 1.15 | 1.5 | **1.4** |
