# 3DTree
This is the Unity Project that turns your Family Tree into a 3D Platformer gaming experience

Prerequisites:
Requires the "Complete Physics Platformer Kit" from Icarus Studio.  Available on the Unity Asset Store.
I have begun to use "ProBuilder Basic" from ProCore.  Available on the Unity Asset Store.
Requires 3DFamilyTreeFileUtilty (github.com/shuskey/3DFamilyTreeFileUtility) in order to read your family tree from FamilySearch.org and
save it as an XML file that 3DTree reads in.  This File Utility will now also simulate family history data now.  I call this the "Adam and Eve, be fruitful and multiply" functionality.


--- 
Over all Concept:
---

Walk, Jump, and Play through your family history!  
3D Family Tree gives a new twist to family history which makes it more personal, approachable and immersive.

Try A VERY EARLY PROTOTYPE here: www.shuskey.com/3dtree/

There is a new twist to 3D family history that has not been tapped yet.  Today 3D Family history seems to be about layering 2D 
charts into 3D space, or mapping out the entire family tree in 3D space as a chart.

3D gaming has some concepts that will make family history more personal, immersive, and approachable.  The data and media become 
the environment; the user controls the avatar trough the environment discovering their family history in a very personal and 
inviting way.  It’s almost like meeting each person individually on a one by one basis, taking time to get to know them. 

My Demo is a static, hand created environment to show the concept only.  I would propose that this environment be generated 
‘on the fly’ by using the user’s FamilySearch credentials and the FamilySearch API to bring in enough information to populate a few 
generations around the Player.  This is known as a “procedural generated environment” in the gaming world.  The entire world does not 
need to be generated, just a few segment ‘in front of’ and behind the avatar are needed.  New segments are generated as the avatar 
moves, ‘left behind’ segments are then removed.

I am drawn to the concept of “walking the tree” as the basis for the game environment, however more artistic approaches might be able
to keep the tree concept, but make it feel less like a tree.  The visualization of a tree with one axis representing time (becoming a 
time line) allows you to seen relationships and missing or incorrect dates very easily.  This is a big bonus!!

Demo 3D Family Tree Explanation:
In my initial concept demo, people in your family tree are represented as a narrow platform that displays their name on the platform,
a picture of them at one end (birth end) and a picture of their tombstone at the other end.  The Z-axis represents time in years, so
the platform also represents a timeline for this person.  A marriage event is represented with a red disk that connects the person to
an adjoining spouse platform.

Person Platform:
The platform is a place that the avatar can visit to get a better view of a persons picture and their tombstone. The platform also 
includes connections to other family members:
-	Step up to get to their parents.
-	Step down to get to younger siblings.
-	Slide down to get to their married life.
-	Move to the adjoining platform (joined with a circle or diamond ring) to get to their spouse.
-	If the person is living, I will have a pumping heart at the end of the platform.

Avatar:
-	Because the avatar is moving in 3D space, I capture the Z-axis position and display it on the screen (upper left) as “YEAR:”

Waypoints above marriage events:
-	Your avatar will re-generated at your last visited waypoint when you fall off the platform or die.

Coins and Green Box enemies:
-	Well, every good game needs something you are collecting and some bad guys trying to stop your progress.  I am not really sure what to do with these, so for now I left them in there to get some brainstorming to occur.

Pickup & Throw Objects:
-	The yellow frame object can be picked up by pressing “K” when you are near it.  It can then be carried around.   Press “K” again and the frame is thrown.

BACK STORY:
I was recently asked to be a Boy Scout Merit Badge Counselor for the new “Game Design” merit badge.  This was very interesting to me 
because of my background in game design.  I developed and sold several computer games back in the 80’s for the Atari ST and the 
Apple II (Yeah pretty 8-bit retro, for sure!).

Anyways, I found that one of the current game developing platforms is called “Unity 3D”.  This is a cross-platform game engine with 
over one million developers. 

Expecting that I would need to teach this to some Boy Scouts, I dove in and created and simple car that I drove around my simulated 
world.  I then ‘re-created’ one of the ‘retro’ games that ran on the Apple II called Flip-Out.  It came together pretty quickly, and 
I was very impressed with the physics engine that allows you create your objects and let them interact with each other just the way 
they would in the real world.

Unity has an awesome “Asset Store” modeled after the Apple “App Store”.  I found a cool “Physics Platform” template – That really 
got me thinking about immersive environments, and what I would do with it.

“Family History” was whispered in my ear one day (Oct 10 2013), and then light broke through the next day (Oct 11 2013) when I 
started creating a static demo of a 3D Family Tree world that my avatar could walk and jump through.

Are you still reading?
January 2015 update:
I have been working over the holidays getting the Family Search GEDCOMx API connected into my Visual Studio C# development environment.
I've started with the initial C# Scripts that I created in Unity3D for the initial demo, and started to merge the two.

I now have made contact with the FamilySearch Sandbox, and am reading in Family Groups into the format I'll use to populate the 3D
world.

In order for you to get this to work, you will need a Developer Key and a Sandbox Login to www.FamilySearch.org

This is just my initial check-in.  I am starting to be concerned about source code and version control, and just the sanity of having a
master copy of this somewhere besides just on my computer.


