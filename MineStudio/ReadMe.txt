Index
1) Installation
2) Application Use

-----------------------------------------------------------------------------

1) Installation
===============
Install GJS Studio by simply double-clicking the GJS Studio.exe file.
Do make sure you install the correct version based on your Windows platform.
In order to use the Programming feature built into GJS Studio, you also need
to install the following two AVR Studios:
- AVR Studio 4 (File Name: aStudio4b-589, filesize = 91Mb)
- AVR Studio 6 (File Name: AStudio61sp1_1net, filesize = 806Mb)

NOTE:  Accept all the Driver installation requests, escpecially Jungo!

If it is already installed on your computer, you can only install GJS Studio.
The order in which these programs are installed is not important.

------------------------------------------------------------------------------

2) Application Use
==================
Please make sure you use COM1 for GJS Studio.

In order to view all available keyboard shortcuts, press the ALT key! Or click
Ctrl + S (or the Shortcuts button) to open the text document showing all
available shortcuts in GJS Studio.
To use the avilable keyboard shortcut, press Ctrl + (desired shortcut key).

Once the Home Page opens, it will automatically look for a VMU or SQ Speedo
to connect to.  

### Connection Hint #################################################
#	It will display that no device is connected if:					#
#	- there is no programmed device connected to the computer,		#
#	- there is a problem with the COMMS of the VMU or SQ Speedo,	#
#	- the device is not properly connected to the computer.			#
#####################################################################

All the configuration and setting buttons will be disabled, until a 
programmed device is connected.  If it finds a device connected, it will 
display the version number in the COUNTS Toolbar.

i) Home Page
============
The following shortcuts will open up other pages available in GJS Studio:
Ctrl + P = opens the Programming Page.
Ctrl + M = opens the Memory Flashes Page.
Ctrl + A = opens the ReadMe text document.
Ctrl + S = opens the Shortcuts text document.

The Home page is designed to view all the different configurations and settings
available on the VMU or SQ Speedo.  If you want to test or view any of these,
follow these steps:

- STEP 1 - Read the VMU or SQ Speedo Version
--------------------------------------------
Ctrl + C (or click the Reconnect Button) in order to read from the VMU or 
SQ Speedo.  When successful, it will display the Version number in the COUNTS 
Toolbar.  This will then load all the configuration and settings available on 
the VMU or SQ Speedo.  If unsuccessful, it will display "No VMU found!".  Refer
to the # Connection Hint # above for possible causes as to why the device might
not connect.
Ctrl + C restarts the Application if there is no VMU connected.

- STEP 2 - Display available Configurations and Settings
--------------------------------------------------------
Ctrl + V (or click the VMU button) in the COUNTS group.  This will refresh 
any VMU or SQ Speedo connected to the computer.
Ctrl + V refreshes the readings of the connected device.

- STEP 3 - GPS
--------------
Ctrl + G (or click the GPS button) in the COUNTS group.  This will display the GPS 
coördinates in the lattitude and longitude boxes.  This is only required for 
Passive Tracking units.

AVAILABLE SETTINGS for the Home Page
------------------------------------
On the Home Page, with a programmed device connected, you can set various settings
on the device.
Set Time - Ctrl + T			Set the VMU Time
Read Time - Ctrl + R		Read the VMU Time
Write Limits - Ctrl + W		Write VMU Limits (feature currently disabled)
Read Limits - Ctrl + E		Write VMU Limits (feature currently disabled)
Turn LED On - Ctrl + N		Turn on 4 x LED's
Turn LED Off - Ctrl + F		Turn off 4 x LED's
Search GPS - Ctrl + G		Get the GPS coördinates
Reconnect - Ctrl + C		Reconnect the VMU, this might restart the Application
Disconnect - Ctrl + D		Disconnect the VMU

ii) OTHER PAGES
===============
- Program Device
----------------
This page is used to program all devices.

Step 1
------
Select which device you want to program from the dropdown box.  If you want to make 
changes to the Bootloader, Flash, etc, click Ctrl + C (or the Change button) to make
changes.  Once you made the necessary changes, click Ctrl + S (or the Save) to save
any changes.

Step 2
------
Click Ctrl + P (or click the Program button).  

ADDITIONALLY, you can Cancel any programming by clicking Ctrl + A (or click the Cancel
Button).

- Memory Flash
--------------
This page will not available if there is no VMU or SQ Speedo connected to the computer.
This page is used to Read (Ctrl + R) and Write (Ctrl + W) the memory on the VMU or 
SQ Speedo.  You can also download (Ctrl + D) individual memory blocks from this page.
The default values are as follows:
Trip blocks = 6			#################################################	
SBS blocks = 13			#	WARNING!	Do not change these values!		#
GPS blocks = 13			#################################################	
