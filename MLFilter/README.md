# MLFilter

A general purpose filter which uses machine learning techniques to process cursor points of up to N samples and M degree of complexity. It can be configured to be used as a latency compensation filter, a low latency cursor correction filter, or a smart smoothing filter.

## Installation

Donwnload the plugins from [latest releases.](https://github.com/X9VoiD/OTDPlugins/actions)

## Configuration Help

These are the different configuration knobs for the filter.

### Offset

Amount of time in milliseconds to offset the prediction for the next point.

* Zero Offset - Filter will apply sub-zero latency cursor correction.
* Positive Offset - Filter will try to predict future cursor position.
* Negative Offset - Filter will delay cursor position to smooth out movement.

> *Over compensating may result in inaccurate and erratic cursor movement.*

### Samples

Determines how long of a history to keep to feed into the filter.

A sample is defined as an update in cursor position.

* `Sample count` is generally a trade-off between CPU usage and accuracy.  

> **Sample must be Degree +1!**

### Complexity

Determines the complexity of prediction to compute by the filter.

> **Higher complexities might require normalization to work correctly.**

### Normalize

Determines whether to convert cursor position scale to a maximum of 1. Helps when using high complexity.
> **_Screen Width_ and _Screen Height_ have to be configured if turning on normalization**

### Averaging Samples

Determines the amount of points to average from the output of the filter
> _Affects latency._

### Feed to Filter

If enabled, averaging will happen _before_ the filter instead of _after._

## Recommended Settings (for 266hz tablets)

| Settings | Value |
| :--- | :--- |
| Compensation | 0 |
| Samples | 20 |
| Complexity | 2 |
| Normalization | Yes |
| Averaging Samples | 0 |
| Feed to Filter | No |
