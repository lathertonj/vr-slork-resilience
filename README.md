# Resilience (SLOrk, 2019)

[Resilience](https://slork.stanford.edu/works/resilience/) is a piece for laptop orchestra and one VR performer / conductor. 
The VR performer's computer simulates a virtual world that seedlings move through.
It is responsible for animation and some timing cues sent automatically to each other laptop.

The code for the VR environment lives here, and the code for making sound in response to performers' gestures
lives in the [SLOrk repository](https://ccrma.stanford.edu/wiki/SLOrk/2016/FAQ).

## Running the Project

While the entire piece can't be run without an absurd setup of networked MacBooks running ChucK code (the SLOrktops),
the VR environment is run through [this scene](Assets/Scenes/V2.unity). It won't do much outside of a performance!

You may be interested in reading code for:

- Seedling animations in [part 1](Assets/Scripts/AnimateSeedlingsPart1.cs), [part 2](Assets/Scripts/AnimateSeedlingsPart2.cs), and 
[part 3](Assets/Scripts/AnimateSeedlingsPart3.cs)
- The [timing and note selector](Assets/Scripts/ModalBarClock.cs), an example of ChucK interoperating with Unity
- The [handler](Assets/Scripts/Part2CommunicateWavingHand.cs) for conductor gestures in the second act of the piece, which translates VR
controller inputs to effects in the virtual world and updates parameters for ChucK code
- Any number of other [scripts](Assets/Scripts)
