import requests
import json
import os
import sys
import tempfile
import datetime

URL = 'https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US'
baseImageURL = 'https://www.bing.com'
SCRIPT_TEMP_DIR = 'auto-wallpaper'
LOG_FILE_NAME = 'auto-wallpaper.log'
TEMP_FILE_LOCATION = ''
LOG_FILE_LOCATION = ''
OS_NAME = ''
DEBUG = False

def WriteLog(message):
	if not os.path.exists(os.path.dirname(LOG_FILE_LOCATION)):
		try:
			os.makedirs(os.path.dirname(LOG_FILE_LOCATION))
		except OSError as exc: # Guard against race condition
			if exc.errno != errno.EEXIST:
				exit()
	file = open(LOG_FILE_LOCATION, "a+")
	now = datetime.datetime.now()
	
	timeStamp = now.strftime("%d.%m.%Y %H:%M:%S")
	file.write(timeStamp + " | " + message + "\n")
	file.close()

if sys.platform.startswith('linux'):
	OS_NAME = 'linux'
	TEMP_FILE_LOCATION = '/tmp/auto-wallpaper/wallpaper.jpg'
	LOG_FILE_LOCATION = '/tmp/auto-wallpaper/' + LOG_FILE_NAME
else:
	import ctypes
	OS_NAME = 'windows'
	SPI_SETDESKWALLPAPER = 20
	osTempDir = tempfile.gettempdir()
	tempDir = os.path.join(osTempDir, SCRIPT_TEMP_DIR)
	TEMP_FILE_LOCATION = os.path.join(tempDir, "wallpaper.jpg")
	LOG_FILE_LOCATION = os.path.join(tempDir, LOG_FILE_NAME)

dataStr = ''
if DEBUG == False:
	WriteLog("Starting wallpaper script")
	response = requests.get(URL)

	if (response.status_code != 200):
		WriteLog("Unable to contact bing.com. Stopping script")
		exit()
	dataStr = response.text
else:
	WriteLog("Starting wallpaper script in Debug mode")
	f = open("test.txt", "r")
	dataStr = f.read()
	f.close()

data = json.loads(dataStr)
imageURL = baseImageURL + data['images'][0]['url']
imageResponse = requests.get(imageURL)

if not os.path.exists(os.path.dirname(TEMP_FILE_LOCATION)):
	try:
		os.makedirs(os.path.dirname(TEMP_FILE_LOCATION))
	except OSError as exc: # Guard against race condition
		if exc.errno != errno.EEXIST:
			exit()
open(TEMP_FILE_LOCATION, 'wb').write(imageResponse.content)

if OS_NAME == 'linux':
	os.system("gsettings set org.gnome.desktop.background picture-uri file://" + TEMP_FILE_LOCATION)
else:
	ctypes.windll.user32.SystemParametersInfoW(SPI_SETDESKWALLPAPER, 0, TEMP_FILE_LOCATION , 0)

WriteLog("Updated wallpaper successfully")
os.remove(TEMP_FILE_LOCATION)