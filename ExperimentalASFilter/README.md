# ExpASFilter

This filter uses machine learning techniques to process cursor points of up to N samples and M degree of complexity. It can be configured to be used as a latency compensation filter, or a low-latency jitter-reduction filter.

# Installation

Donwnload the plugins from [latest releases.](https://github.com/X9VoiD/OTDPlugins/actions)

# Configuration Help

These are the different configuration knobs for the filter.


## Compensation
Amount of time in milliseconds to compute into the future. Reduces perceived latency upon proper configuration.
> *Over compensating may result in inaccurate and erratic cursor movement.*    
> **Gets overridden by [Report Rate Synchronization](https://github.com/X9VoiD/OTDPlugins/wiki/ExperimentalASFilter#synchronize-to-report-rate)**
   

## Samples
Determines how long of a history to keep to feed into the filter.    

A sample is defined as an update in cursor position.    

* `Low sample count` means faster response to a cursor's velocity/acceleration (check [Degree](https://github.com/X9VoiD/OTDPlugins/wiki/ExperimentalASFilter#degree)) but results to discontinuous cursor movement.    
* `High sample count` means better cursor continuity but at the cost of possibly increasing latency when applied in conjunction to incompatible configurations.    
    
> **Sample must be Degree +1!**    
> *Good sample count for first degree is 5-8, second degree is 6-10*


## Degree
Determines the complexity of prediction to compute by the filter.    
    
First degree complexity (detects and predicts future velocity)    
Second degree complexity (detects and predicts future acceleration)    
> **Higher degrees are not recommended to use.**
    

## Normalize
Determines whether to convert cursor position scale to a maximum of 1. May or may not have any effect on the filter.    
> **_Screen Width_ and _Screen Height_ have to be configured if turning on normalization**
    

## Synchronize to Report Rate
Overrides then calculates [Compensation](https://github.com/X9VoiD/OTDPlugins/wiki/ExperimentalASFilter#compensation) using the report rate of the connected tablet.    
> _Improves cursor continuity._    
> **Requires Reports Ahead**
    

## Reports Ahead
Calculates [Compensation](https://github.com/X9VoiD/OTDPlugins/wiki/ExperimentalASFilter#compensation) with:
> `Report Rate * Reports Ahead`   
    
> Effectively predicts a future cursor position when filter is configured properly.    
    

## Exponential Weighted Polynomial Regression
Exponentially prioritize more recent cursor point samples in efforts of reducing latency when filtering with high sample counts.    
    

# Recommended Settings (for 266hz tablets)

| Settings | Value |
| :--- | :--- |
| Compensation | n/a |
| Samples | 6 |
| Degree | 1 |
| Synchronize | Yes |
| Reports Ahead | 8 |
| Exponential | Yes |
