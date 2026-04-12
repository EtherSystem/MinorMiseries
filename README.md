# Minor Miseries

Minor Miseries is a mod for The Long Dark that introduces a layer of everyday physical discomforts.

These afflictions are not lethal. they are minor and easy to ignore.

*Until they aren’t anymore.*

Small problems accumulate. Neglect has consequences. Comfort is never permanent.

Minor Miseries now also includes a few situational protective buffs, allowing the player’s gear and recent experience to influence which miseries can occur.

ALSO A HUGE THANK YOU TO FLOWER FIELD FOR THESE MAGNIFICENT ICONS !

## Overview

Minor Miseries adds a collection of non-fatal afflictions that trigger under specific gameplay conditions.

Some afflictions can evolve if left untreated, becoming more severe or introducing additional risks.

The system is built around escalation, persistence, cumulative pressure, and context-sensitive risk rather than sudden punishment.

Certain risks are now influenced by the tools you use, while some pieces of protective clothing can help prevent specific afflictions at the cost of item condition.

## Afflictions

**Blister**  
Appears after prolonged walking or sprinting. Slightly reduces mobility.  
Can evolve into **Bare Skin** if ignored.

**Bare Skin**  
Represents worn and exposed skin caused by repeated friction. Strongly reduces movement efficiency and carries a risk of infection if neglected.

**Back Pain**  
Triggered by extended encumbrance. Reduces carrying capacity.  
This affliction overrides carry weight values and *may* conflict with other mods that modify carry capacity.

**Splinter**  
Can occur during breakdown actions with unprotected hands.  
The chance depends on the tool being used.  
Can evolve into **Sensitive Hand**.

**Sensitive Hand**  
Develops from an untreated splinter. Further reduces crafting and rope climbing efficiency.

**Scratch**  
Can occur while crafting with unprotected arms.  
The chance depends on the tool being used.  
Can evolve into **Small Cut**.

**Small Cut**  
Develops from an untreated scratch. Introduces a risk of infection if neglected.

**Wrist Trauma**  
Can occur when firing a revolver with low firearm skill.  
Makes aiming and weapon handling more difficult, and also affects crafting and rope climbing.

**Shoulder Trauma**  
Can occur when firing a rifle with low firearm skill.  
Makes aiming and weapon handling more difficult, and also affects crafting and rope climbing.

**Stuck Food**  
Can occur after eating. Causes **extreme** discomfort.  
(Currently may also trigger from calorie-giving drinks — unintended behavior.)

**Sore Neck**  
Can occur after sleeping inside a car or a snow shelter.  
Sleeping in a car is riskier than sleeping in a snow shelter.

**Bad Dream / Night Terror**  
May occur during sleep if the player has recently been attacked by wildlife.  
Abruptly wakes the player and interrupts rest.

Each type of wildlife struggle contributes differently to the severity of the stress that builds up:

Wolves -> Moose -> Cougar -> Bear

The more intense the accumulated stress, the greater the chance of suffering **Night Terror** instead of **Bad Dream**.

Keep in mind that **Night Terror** always lasts twice as long as **Bad Dream**.

## Buffs

**Protected Hands**  
Wearing appropriate gloves in good enough condition grants protection against **Splinter** during breakdown actions.  
While the buff is active, **Splinter** cannot develop, but the gloves will lose condition each time a **Splinter** would have appeared.

**Protected Arms**  
Wearing sturdy upper-body clothing in good enough condition grants protection against **Scratch** while crafting.  
While the buff is active, **Scratch** cannot develop, but the clothes will lose condition each time a **Scratch** would have appeared.

**Peace Of Mind**  
If enough time passes since your last wildlife attack and you manage to get proper sleep, you may gain **Peace Of Mind**.  
While active, **Bad Dream** and **Night Terror** can no longer occur.  
This buff is lost if wildlife stress returns.

## Overconfidence System

If the player goes too long without suffering any affliction, they develop a growing **Overconfidence Risk**.

If this risk reaches its maximum, it evolves into **Overconfidence**.

While Overconfidence is active:

- The probability of Minor Miseries afflictions increases
- Some afflictions trigger faster or hit harder
- The severity of the first triggered affliction is amplified

The mod is fundamentally built around this mechanic.

## Customization

All major afflictions can be individually enabled or disabled.

Durations are configurable.

Evolving afflictions are grouped under their base affliction for clearer setup.

The **Peace Of Mind** threshold is configurable.

For intended balance and design cohesion, keeping **Overconfidence** enabled is strongly recommended.

## For Developers
<details>
<summary><strong>Click to Expand</strong></summary>

### ModSetting

An advanced setting allows to modify rare alternative icons chance to appear for afflictions and buffs.

### Console Commands

The following debug commands are available via the [Developer Console](https://github.com/DigitalzombieTLD/TLD-Developer-Console/).  
These commands are intended strictly for testing and balancing.

---

### Trigger ALL Afflictions

**mm_afflictions**  
Applies the main Minor Miseries afflictions for testing purposes.

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

**nightterror**  
Triggers Night Terror.

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

**soreneck**  
Triggers Sore Neck.

---

### Overconfidence System Commands

**overcrisk**  
Applies Overconfidence Risk.

**overc**  
Applies Overconfidence.

**overc_cure**  
Cures both Overconfidence Risk and Overconfidence if present.

</details>

## Installation

1. Install MelonLoader.
2. Install [AfflictionComponent](https://github.com/TLD-Mods/AfflictionComponent), [ModComponent](https://github.com/dommrogers/ModComponent), [ModSettings](https://github.com/DigitalzombieTLD/ModSettings/) and [ModData](https://github.com/dommrogers/ModData).
3. Place `Minor_Miseries.dll` inside your Mods folder.
