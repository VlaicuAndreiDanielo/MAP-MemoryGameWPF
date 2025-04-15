## MAP-MemoryGameWPF
# 🧠 Memory Game

A **Memory Game** developed using the **MVVM architecture**, built with flexibility, personalization, and fun in mind. The game challenges players to match image pairs and improve their memory through various levels of difficulty, categories, and themes.

---

## 🎮 Game Features

### 🗂️ Categories

Players can choose from the following image categories:

- Animals  
- Flowers  
- Trees  
- Vegetables  
- Fruits  
- Rocks  
- Landscapes  
- Buildings  
- Motorcycle  
- Cars  
- Tools  
- Random → selects images from **all categories**

If a selected category does not yet have assigned cards, a placeholder image like _"Sorry, not added yet"_ will appear.

---

### ⚙️ Game Modes

Multiple difficulty levels are available to challenge the player’s memory:

- Baby Mode  
- Easy Mode  
- Medium Mode  
- Intermediate Mode  
- Hard Mode  
- Very Hard Mode  
- Challenging Mode  
- Expert Mode  
- Nightmare Mode  
- Hell Mode  
- Insane Mode  
- God Mode  
- Custom Mode → allows **manual input** for game settings

Only **Custom Mode** allows the player to manually adjust:
- Number of rows  
- Number of columns  
- Time limit

---

### 🧩 Randomization Logic

- Images are randomly chosen from the selected category.
- Pair matching is randomized each time the game starts.
- If the total number of needed images is larger than the available pool, some images may **repeat**.

---

### 💾 Save & Resume System

- Players can **save a game in progress**.
- If the game is **won** or **deleted manually**, it is removed from memory.
- If the player **quits**, the game is kept in memory and can be resumed later.

---

## 🪟 Application Windows

The app includes several main user interfaces:

### 1. **Game Setup Window**
- Allows users to choose category, game mode, and (if in custom mode) configure rows, columns, and timer.
- Theme selector is also available here.
- **Close button is disabled** to avoid accidental exits.

### 2. **Game Window**
- Displays the card grid with a countdown timer (if applicable).
- Cards flip on click to reveal images.
- Game ends when all pairs are matched or time runs out.
- **Close button is disabled**.

### 3. **Game Won Window**
- Shown when the player wins the game.
- Displays time taken and statistics.
- Option to play again or return to main menu.
- **Close button is disabled**.

### 4. **Game Lost Window**
- Appears if the player runs out of time.
- Option to retry or return to main menu.
- **Close button is disabled**.

---

## 🎨 Color Themes

Users can personalize the visual style of the game using pre-defined themes, all built with XAML resource dictionaries:

- `BabyPinkTheme.xaml`  
- `DeepBlueTheme.xaml`  
- `DeepOrangeTheme.xaml`  
- `ForestGreenTheme.xaml`  
- `HeavenLightTheme.xaml`  
- `IntenseVioletTheme.xaml`  
- `PaleLavenderTheme.xaml`  
- `QuantumRedTheme.xaml`  
- `SunnyYellowTheme.xaml`  
- `TotalDarknessTheme.xaml`  

Themes apply instantly to the entire UI and enhance the user experience through visual customization.

---

## 🧠 MVVM Architecture

This project follows the **MVVM (Model-View-ViewModel)** pattern for a clean separation of concerns:

- **Model** → represents the game data (cards, categories, saved states, etc.)
- **View** → the user interface (XAML-based windows for Game, Setup, etc.)
- **ViewModel** → the logic that binds the View to the Model (e.g., handling game logic, commands, property binding)

### Advantages:
- Easier to test
- Scalable and maintainable
- Cleaner code and separation between UI and logic

---

## 🧪 Additional Info

- All card images are handled programmatically and loaded dynamically.
- Themes and UI are scalable for different resolutions.
- If future image categories are added, they can easily be integrated into the game.
- The game is designed to be lightweight and responsive.

---

💡 **Tip**: For best experience, play in full screen with a high-difficulty mode and random category!

---

Enjoy matching and sharpening your memory! 🧠🃏🔥
