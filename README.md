
# Minor Miseries

Minor Miseries is a mod for The Long Dark that introduces a layer of everyday physical discomforts.

These afflictions are not lethal, they are minor and easy to ignore.  

*Until they aren’t anymore.*

Small problems accumulate. Neglect has consequences. Comfort is never permanent.

## Overview

Minor Miseries adds a collection of non-fatal afflictions that trigger under specific gameplay conditions.

Some afflictions can evolve if left untreated, becoming more severe or introducing additional risks.

The system is designed around escalation, persistence, and cumulative pressure rather than sudden punishment.

ALSO A HUGE THANK YOU TO FLOWER FIELD FOR THESE MAGNIFICENT ICONS !

## Afflictions


**Blister**  
Appears after prolonged physical exertion. Slightly reduces mobility.  
Can evolve into **Bare Skin** if ignored.

**Bare Skin**  
Represents worn and exposed skin. Applies stronger movement penalties and carries a risk of infection.

**Back Pain**  
Triggered by extended encumbrance. Reduces carrying capacity.  
This affliction overrides carry weight values and may conflict with other mods that modify carry capacity.

**Splinter**  
Can occur during manual actions without protection. Reduces crafting and rope climbing speed.  
Can evolve into **Sensitive Hand**.

**Sensitive Hand**  
Develops from an untreated splinter. Further reduces crafting and climbing efficiency.

**Scratch**  
May occur while crafting. Slightly reduces crafting speed.  
Can evolve into **Small Cut**.

**Small Cut**  
Develops from an untreated scratch. Introduces a risk of infection if neglected.

**Wrist Trauma**  
Can occur when firing a revolver with low firearm skill.  
Reduces crafting speed, rope climbing speed, and shortens maximum rifle aim duration.

**Shoulder Trauma**  
Can occur when firing a rifle with low firearm skill.  
Reduces crafting speed, rope climbing speed, and shortens maximum rifle aim duration.

**Food Stuck**  
Can occur after eating. Causes **extreme** discomfort.  
(Currently may also trigger from calorie-giving drinks — unintended behavior.)

**Food Stuck**  
Can occur after sleeping inside a car (60% chance) or a snow shelter (40% chance).

**Bad Dream / Night Terror**  
May occur during sleep if the player has recently been attacked by wildlife.
Abruptly wakes the player and interrupts rest.
Each type of attack varies in severity, the more attacks the player has suffered, the greater the chance of contracting Night Terror instead of Bad Dream.

Here is the order of severity of predator attacks: Wolves -> Moose -> Cougar -> Bear.

## Overconfidence System

If the player goes too long without suffering any affliction, they develop a growing **Overconfidence Risk**.

If this risk reaches its maximum, it evolves into **Overconfidence**.

While Overconfidence is active:

- The probability of all other Minor Miseries afflictions increases
- The severity of the first triggered affliction is amplified

The mod is fundamentally built around this mechanic.


## Customization

All afflictions can be individually enabled or disabled.

Durations are configurable.

For intended balance and design cohesion, keeping Overconfidence enabled is strongly recommended.

## For Developers
<details>
<summary><strong>Click to Expand</strong></summary>

### Console Commands

The following debug commands are available via the [Developer Console](https://github.com/DigitalzombieTLD/TLD-Developer-Console/).  
These commands are intended strictly for testing and balancing.

---

### Trigger ALL Afflictions

**mm_afflictions**  
Applies all Minor Miseries afflictions for testing purposes.

**mm_afflictions_cure**  
Cures all Minor Miseries afflictions currently active.

---

### Individual Affliction Commands

**splinter**  
Triggers Splinter.

**stuckfood**  
Triggers Food Stuck.

**blister**  
Triggers Blister.

**backpain**  
Triggers Back Pain.

**scratch**  
Triggers Scratch.

**baddream**  
Triggers Bad Dream.

**sensitivehand**  
Triggers Sensitive Hand.

**bareskin**  
Triggers Bare Skin.

**smallcut**  
Triggers Small Cut.

**wristrecoil**  
Triggers Wrist Trauma.

**shoulderrecoil**  
Triggers Shoulder Trauma.

---

### Overconfidence System Commands

**overcrisk**  
Applies Overconfidence Risk.

**overc**  
Applies Overconfidence.

**overc_cure**  
Cures both Overconfidence Risk and Overconfidence if present.

</details>


## Visual Assets

Custom affliction sprites are planned for Minor Miseries.

At the moment, technical limitations prevent a clean and stable implementation.

This feature is intended for a future update.


## Installation

1. Install MelonLoader.
2. Install [AfflictionComponent](https://github.com/TLD-Mods/AfflictionComponent), [ModComponent](https://github.com/dommrogers/ModComponent) and [ModSettings](https://github.com/DigitalzombieTLD/ModSettings/).
3. Place `Minor_Miseries.dll` inside your Mods folder.
