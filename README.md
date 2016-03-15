# GuardianSpaceFighter

This is the Space Guardian Fighter game I developed for stage 2 of the Search for a star 2016 competition.
It is based on an initial project (which is available in the InitialProject branch), which I have modified quite a lot to improve the game as much as I could.

# Gameplay
I made quite a number of changes in the gameplay. To start with, the initial project limited the player to 3 different lanes, between which he could switch by swiping left or right. While I can understand the appeal in a mobile game, I felt that in the context of my game, that was too limiting for the player and a finger controlled character would still play very well on mobile.
The biggest change I made was the addition of powerups. Those can be bought from the store for currencies, and genuinely improve the player’s fighting capacity. Here is the list of powerups :
-	Multishot – this increases the number of bullets the player shots
-	Bulletspeed – this increases the speed of the bullets shot by the player. It can be surprisingly important because it also reduces the spread of the bullets when the player has some multishot levels
-	Firerate – this increases the frequency at which the player can shoot
-	Sidelaser – this reduces the cooldown and increases the damage of the SideLaser (you need at least one point in SideLaser to use it)
In order to keep track of the player’s upgrades and other properties, I create a UserData class. This is a persistent class which does not get deleted when you switch scenes. It stores all the information about the player and saves it in the PlayerPrefs if the player leaves the game. When the game then starts, it reloads those information from the PlayerPrefs. 
![SaveLoad](/GitHubPics/SaveLoad.PNG?raw=true)



# Points system
The score system is quite an important an unusual part of the game. Each time you kill an enemy, you acquire some points. Enemies can also sometimes spawn powerups worth 1000 points on death. When you complete a level, the numbers of points that you have obtained through this level will be added to your points available for use on the shop if you have never completed that level before. If you did complete it before however, and already have a best score, then only the difference between your previous best score and your current score will be added.
As an example, if I score 2500 points but my previous best score was 2000 points, 500 points will be added to my total. If I scored 1500 points instead, 0 points will be added.
The logic behind this system is that I want the player to try to replay previous levels after gaining a few powerups to try to gain more currency. The levels are designed in such a way that it is almost impossible to clear them entirely on the first run – actually it is a very good strategy to kill a few enemies to gain points and then hide in a safe corner to validate the level.
With this system, the player is motivated to try to beat his previous best scores, but will not just farm a single level continuously to get more and more points.
![Shop - GUI](/GitHubPics/shop.PNG?raw=true)

# Overworld
I have created an overworld which is navigated by swiping left or right. Your avatar in the overworld will the move automatically towards the next level, which you can enter by taping the screen. You are also able to enter the shop from the overworld, which allows you to spend your different currencies to buy items.
I have created a function which checks if your tap is inside of the bounds of a collider and I am using it to detect when the player clicks on a button in the shop.
In addition I implemented a real money currency. It is setup with the Windows Store and uses Unity’s Store Listener. It allows player to spend real money to buy upgrades early.
![Shop - Code](/GitHubPics/shop2.PNG?raw=true)

# The Windows Store
I published my game on the Windows Store, as was requested by the Search for a Star competition. As I had not done so before it did take me quite a bit of trial and error to figure out how to do it, however I was expecting it to take some time so I had made sure to leave some time for this.


# Conclusion
There are multiple improvements I could be doing on my game. The most obvious one is of course to make it available on Android and Apple devices. Additionally, there are a number of other powerups that could be added to simplify the game a bit for the player, such as : Increased HP (or reduce damage), Forward laser (Might be hard to balance), ship speed, temporary shield, etc
In addition, I could add plenty of content. The game is setup in such a way to generating new levels is extremely easy for me, although I would need to add some enemy variety to spice up the game. Long term I might want to have several bosses on the screen at once which would require some changes in the code.
![Levels](/GitHubPics/levels.PNG?raw=true)



Link towards a video of the game running : https://youtu.be/MAhrqgllTtU

Link towards the game in the Windows Store : https://www.microsoft.com/en-us/store/games/guardian-space-fighter/9nblggh5m6t4
