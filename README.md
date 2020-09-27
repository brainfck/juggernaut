# juggernaut
 NASA Image of the Day Desktop Downloader

This is a short C# program that, when run, writes [NASA image of the day](https://www.nasa.gov/multimedia/imagegallery/iotd.html) to image file.  You can use those images to create a slideshow. 

See [Releases](../../releases/) page for the working builds!

# using it

I have a shortcut to the program in my startup folder on my Windows 10 PC, so it automatically downloades the latest image daily.

If you want the images stored in a location other than the EXE directory, put the batch files in your startup folder instead, and modify the two paths listed at the top:
```batch
REM The runnable file
set "exec_location=[EXE location]"
REM The directory to store the images in. Don't use quotes here.
set "image_path=[IMAGE_LOCATION]"
```

# how it works?

This works by accessing NASA Image of the Day RSS feed, gets the image location, then downloads the image to specified directory. 
If no directory is specified it downloades image to same directory, from which you run the program.