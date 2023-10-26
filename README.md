# Signal Booster for Dead Signal

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Neonalig/Signal-Booster?style=flat-square)](<https://github.com/Neonalig/Signal-Booster/releases/latest>)
[![GitHub](https://img.shields.io/github/license/Neonalig/Signal-Booster?style=flat-square)](<https://github.com/Neonalig/Signal-Booster/blob/main/LICENSE.md>)
[![GitHub all releases](https://img.shields.io/github/downloads/Neonalig/Signal-Booster/total?style=flat-square)](<https://github.com/Neonalig/Signal-Booster/releases/latest>)

Welcome to **Signal Booster**, the must-have companion for every avid player of the original challenging version of [Dead Signal](https://store.steampowered.com/app/2599300/Dead_Signal/). Struggle no longer with that uniquely challenging security keypad! 🎮🔧

Let's face it, we've all been there - facing off a hundred different threats, and then BAM (quite literally), as you're shown off by Lucas because you happened to be looking at the computer the same frame the code changed. But fear not, brave (or foolish) gamer, for **Signal Booster** is here to turn your luck around bypass all those dead signals or whatever it is you do in that game.

## What is Signal Booster?

**Signal Booster** is a simple, yet devilishly clever C# WPF application that acts as your trusty sidekick in the tumultuous world of [Dead Signal](https://store.steampowered.com/app/2599300/Dead_Signal/). It's a rolling timer that syncs up with the game's quirky keypad code mechanic, helping you predict when the next code change will occur and giving you that much-needed edge to conquer the game (or at least not die as often 💀).

Powered by [lepo.co WPF UI](https://github.com/lepoco/wpfui), **Signal Booster** boasts a modern Windows 11-style fluent design that is as sleek as it is user-friendly. It's like the Batman to your Gotham - you didn't know you needed it until it showed up. And it only marginally makes things worse!

## Features

- **Rolling Timer**: Syncs up with the game's keypad code mechanic, helping you track when the next code change will happen.
- **Keybinds**: Two simple keybinds to rule them all!
  - One key to mark when a code changes, allowing the rolling timer to adjust and become more accurate with each subsequent code change.
  - A second key for pausing or clearing the timer when, not if, you meet your untimely demise in the game.
- **Sleek Design**: Utilises [lepo.co WPF UI](https://github.com/lepoco/wpfui) for a modern, stylish interface that will make your gaming setup look even cooler, 100% guaranteed or your money back!*
- **Easy to Use**: So simple, even a gamer who needs the help of external tools can use it! (no offense.)

(*Not actually guaranteed. It's literally free. You can't get a refund if you don't pay anything. But it's still pretty cool.)

Actually, speaking of payment, [buy me a cuppa?](https://bmc.link/neonalig) ☕👀

## Getting Started

1. Download the latest release of **Signal Booster** from the [releases page](https://github.com/Neonalig/Signal-Booster/releases).
2. Extract the contents of the zip file to a folder of your choice.
3. Boot up the game and open **Signal Booster** to the side (or on a second monitor if you're a real cool kid).
4. Use the designated keybinds to interact with **Signal Booster** while playing the game.
   - By default, `NumPad -` will mark when the code changes, and `NumPad *` will pause or clear the timer.
   - Press `NumPad -` whenever the keypad code changes in the game. The first time you press it, the timer will enter a calibration mode, and waits patiently for your next input. Press it again when the code changes to mark the new code, and the timer will adjust accordingly and begin rolling. Should it be miscalibrated or slightly out-of-sync, simply press `NumPad -` again when the code actually changes to correct it.
   - Press `NumPad *` if you need to pause the timer (`Numpad -` will resume). When paused, if you press `NumPad *` again, the timer will be cleared and reset for a fresh start.
5. Revel in the glory of your newfound gaming prowess.

## Keybinds

- **Rollover**: Press `NumPad -` (you can change this in the settings) whenever the keypad code changes in the game.
- **Cancel**: Press `NumPad *` (this one is customisable too) when you die (because let's be honest, it's going to happen) to pause or clear the timer and start afresh.
- All keybinds can be customised with the keybind editor at the bottom right of the main window. Keybinds are stored to a local `keybinds.json` file in the same directory as the application to maintain your customisations between sessions and stay portable.

## Contributing

Found a bug? Have a feature request? Or just want to show off your high score? Feel free to [open an issue](https://github.com/Neonalig/Signal-Booster/issues) or submit a pull request (for everything except that last one)! Your feedback makes **Signal Booster** faster, stronger, and even more awesome!

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

Big shout out to [Reflect Studios](https://www.reflectstudios.com/) for their amazing games! Without them, this very literally would not be possible. If you haven't already, go check out their games, they're awesome!

Additional shout out to [lepo.co WPF UI](https://github.com/lepoco/wpfui) for the snazzy interface. It's like the icing on the cake, but without the calories. 🍰

And of course, a massive thank you to all the gamers out there, relentlessly cracking codes and dying in hallways - you're the real MVPs. 🏆

Now go out there and boost all those dead signals like a pro! 💪🎭
