<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <h2 align="center">Sounity</h2>

  <p align="center">
    A nearly dependency-free package for custom positional sounds.
    <br />
    Built with <a href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API">WEB AUDIO API</a>
    <br />
    <br />
    <br />
    <a href="https://www.youtube.com/watch?v=Zru5S0NzmpA">PREVIEW</a>
    ·
    <a href="https://github.com/araynimax/sounity/issues">Report Bug</a>
    ·
    <a href="https://github.com/araynimax/sounity/issues">Request Feature</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li><a href="#important-note-at-the-start">Important Note at the start</a></li>
    <li><a href="#features">Features</a></li>
    <li><a href="#upcoming-features">Upcoming features</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#example-usages">Examples</a></li>
    <li><a href="#api">API</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->

## About The Project

It started as test project to understand scripting for gta-server more.
Since I didn't find any resources that feature a real 3d sound, I've started creating my own resource.

I tried my best to make it as simple as possible for potential user of this resource.

### Important Note at the start

You might experience some "crackling" but it should not be the norm.
That crackling is a chromium bug that is under investigation: https://bugs.chromium.org/p/chromium/issues/detail?id=1242647
I've tested a lot and with all the background noises of the game, I've heared very little crackings that weren't that loud.

If you have bigger issue with that cracklings than I'm sorry to say that you have to wait for a bugfix.

### Features

- Positional sound
- Positional sound attached to an entity.
- Server-side created sounds are time-synced on every client
- create and use custom Filter/Effects
- Underwater effect
  - If the sound source and/or the listener are underwater the sound gets muffled

### Upcoming Features

You have an idea?
Submit it [here](https://github.com/araynimax/sounity/issues).

### Installation

1. Take a look into the [release section](https://github.com/araynimax/sounity/releases) and download the newest version of sounity
2. Unzip the Zip-file in your resource folder
3. Add `ensure sounity" to your server.cfg
4. Play sounds

<!-- USAGE EXAMPLES -->

## Example Usages

Basic example of using this library.
For a detailed list of all methods scroll down to [#API](#api)

### Play a sound on a specific location (lua example)

```lua
-- create a sound instance
local soundId = exports.sounity:CreateSound("URL TO MP3", json.encode({
    posX = 3,
    posY = 5,
    posZ = 8,
}));

-- start playback of that sound
exports.sounity:StartSound(soundId);
```

### Create a sound and attach it to the player (lua example)

```lua
-- create a sound instance
local soundId = exports.sounity:CreateSound("URL TO MP3");

-- attach it to the player ped
local ped = GetPlayerPed(source);
local pedId = NetworkGetNetworkIdFromEntity(ped);

exports.sounity:AttachSound(soundId, pedId)

-- start playback of that sound
exports.sounity:StartSound(soundId);
```

## API

The API is **nearly** identical on server and client.<br/>
The biggest difference is, that on client side there is no sync to other players.<br/>
For the most cases I recommend a server side usage.

### Methods

All Methods are available through [exports](https://docs.fivem.net/docs/scripting-manual/runtimes/javascript/#using-exports)

#### `CreateSound(source: string [, options: json_string]):string`

Creates a new sound instance and returns the sound's identifier.<br/>

##### options

| name(key)        | description                                                                                                                                       | possible values                          | default value |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------- | ------------- |
| `volume`         | Defines volume in percent                                                                                                                         | `positive numbers`                       | `1`           |
| `outputType`     | Choose which volume setting will be used                                                                                                          | `'sfx' \| 'music'`                       | `'sfx'`       |
| `loop`           | If true the sound will repeat until you stop it                                                                                                   | `bool`                                   | `false`       |
| `posX`           | position on x axis                                                                                                                                | `float`                                  | `0`           |
| `posY`           | position on y axis                                                                                                                                | `float`                                  | `0`           |
| `posZ`           | position on z axis                                                                                                                                | `float`                                  | `0`           |
| `rotX`           | rotation on x axis                                                                                                                                | `float`                                  | `0`           |
| `rotY`           | rotation on y axis                                                                                                                                | `float`                                  | `0`           |
| `rotZ`           | rotation on z axis                                                                                                                                | `float`                                  | `0`           |
| `panningModel`   | defines the spatialisation algorithm read more [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/panningModel)                   | `'equalpower' \| 'HRTF' `                | `'HRTF'`      |
| `distanceModel`  | defines the algorithm reducing the volume on distance read more [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/distanceModel) | `'linear' \| 'inverse' \| 'exponential'` | `'inverse'`   |
| `maxDistance`    | this value is only used by the linear distanceModel read more [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/maxDistance)     | `positive numbers`                       | `500`         |
| `refDistance`    | this value is used by all distance models read more [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/refDistance)               | `positive numbers`                       | `3`           |
| `rolloffFactor`  | this value is used by all distance models read more [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/rolloffFactor)             | `on linear: 0-1, others: 0-INFINITY`     | `1`           |
| `coneInnerAngle` | inner angle of sound without sound reduction [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/coneInnerAngle)                   | `float (degree)`                         | `360`         |
| `coneOuterAngle` | outer angle with a volume set to a defined constant [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/coneOuterAngle)            | `float (degree)`                         | `0`           |
| `coneOuterGain`  | volume in outer angle (0 means no sound at all) [here](https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/coneOuterGain)                 | `0-1`                                    | `0`           |

#### `StartSound(identifier: string): void`

Starts the playback of a sound

#### `MoveSound(identifier: string, x: float, y: float, z: float): void`

Set a new location for a sound
**This does not work if an entity is attached!**

#### `RotateSound(identifier: string, x: float, y: float, z: float): void`

Set a orientation for a sound
**This does not work if an entity is attached!**

#### `StopSound(identifier: string): void`

Stops playback of a sound

#### `AttachSound(identifier: string, entity_networkid: int): void`

Attachs a sound to an entity

#### `DetachSound(identifier: string)`

Detachs a sound from an entity

#### `DisposeSound(identifier: string)`

Destroys a sound

#### **[CLIENT ONLY]** `CreateFilter(filterName: string, filterType: 'biquad' | 'convolver', options: json_string)`

Create a filter with a unique name.

You can choose between a biquad filter or a convolver filter.

##### filterType: 'biquad'

Biquad is a low-order filter.
The options are identical to the [official documentation](https://developer.mozilla.org/en-US/docs/Web/API/BiquadFilterNode)

##### filterType: 'convolver'

A more complex filter type is the convolver. This filter is often used to get a reverb-effect.
You need a impulse-response file.

[official documentation](https://developer.mozilla.org/en-US/docs/Web/API/ConvolverNode)

[free reverb impulse responses](https://www.voxengo.com/impulses/)

###### options

- url: string
  - url to impulse response
- disableNormalization: bool
  - A boolean value controlling whether the impulse response from the buffer will be scaled by an equal-power normalization, or not. The default is 'false'.

#### `AddSoundFilter(identifier: string, filterName: string)`

Add a filter to a sound

#### `RemoveSoundFilter(identifier: string, filterName: string)`

Remove a filter from a sound

#### **[SERVER]** `AddListenerFilter(playerId: int, filterName: string)`

#### **[CLIENT]** `AddListenerFilter(filterName: string)`

Add filter to listener. (Global Filter on client)

#### **[SERVER]** `AddListenerFilter(playerId: int, filterName: string)`

#### **[CLIENT]** `RemoveListenerFilter(filterName: string)`

Remove filter from listener.

<!-- CONTRIBUTING -->

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->

## License

Distributed under the MIT License. See `LICENSE` for more information.

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[forks-shield]: https://img.shields.io/github/forks/araynimax/sounity?style=for-the-badge
[forks-url]: https://github.com/araynimax/sounity/network/members
[stars-shield]: https://img.shields.io/github/stars/araynimax/sounity?style=for-the-badge
[stars-url]: https://github.com/araynimax/sounity/stargazers
[issues-shield]: https://img.shields.io/github/issues/araynimax/sounity?style=for-the-badge
[issues-url]: https://github.com/araynimax/sounity/issues
[license-shield]: https://img.shields.io/github/license/araynimax/sounity?style=for-the-badge
[license-url]: https://github.com/araynimax/sounity/blob/main/LICENSE.txt
