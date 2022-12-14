# Wall System

## Intro

After having been inspired by my group projects, I went ahead and implemented a very similar variation to a 'road system' I implemented. I started from scratch rather then try to cut out my already existing code since I felt this would be easier and less work in the end. This project has been mainly about rewriting and implementing common gameplay mechanics that utilize algorithms (e.g. depth first search). 

At the time of writing this not exactly every mechanic is present that I want to be present since this is a work in progress still, I however don't doubt my ability to easily be able to expand and add all the listed mechanic given some time. 

![image](https://user-images.githubusercontent.com/40210931/200935211-2a12a793-2361-4702-85da-81deb4ca62c5.png)


## Mechanics

### Building mechanic

The building mechanic works fairly simple and uses a raycast to project and snap wall objects to a tile grid. Basically, I generate a square grid layout with each 'tile' being a Tile class. Every Tile contains 4 edges and 4 corners and every edge also has 2 corners to help the wall system generate visuals.

 ![image](https://user-images.githubusercontent.com/40210931/203131446-9e08aa9e-7262-4c9b-98fc-157487f9e0ed.png)


### Wall system
The wall system is the main focus of this project, it gets utilized by the builder to check and store every wall that gets placed to procedurally update corner pieces whenever a change in wall placement occurs. For example placing 2 walls next to eachother will spawn a corner inbetween, this will ofcourse work the other way around. 
![image](https://user-images.githubusercontent.com/40210931/203131595-aa007b51-b2dc-4485-bef1-4065361da2de.png)


## Credits

### External code
Since camera movement wasn't the focus of this project, I used a simple camera movement script that I found online: https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996

### Packages
For the visuals of the walls I donwloaded Walls and Floors v1 and for the ground texture I used the Outdoor Ground Textures asset pack

