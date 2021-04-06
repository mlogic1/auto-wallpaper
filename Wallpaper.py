import requests
#from bs4 import BeautifulSoup
import json
import os
import sys

URL = 'https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US'
baseImageURL = 'https://www.bing.com'
TEMP_FILE_LOCATION = ''

if sys.platform.startswith('linux'):
    TEMP_FILE_LOCATION = '/tmp/auto-wallpaper/wallpaper.jpg'
else:
    TEMP_FILE_LOCATION = '%temp%/auto-wallpaper/wallpaper.jpg'
exit()

#response = requests.get(URL)

#if (response.status_code != 200):
#    print("Unable to contact bing.com")
#    exit()

f = open("test.txt", "r")
test_json_text = f.read()
f.close()
data = json.loads(test_json_text)
#print(data['images'][0]['url'])
imageURL = baseImageURL + data['images'][0]['url']
print(imageURL)
imageResponse = requests.get(imageURL)

if not os.path.exists(os.path.dirname(TEMP_FILE_LOCATION)):
    try:
        os.makedirs(os.path.dirname(TEMP_FILE_LOCATION))
    except OSError as exc: # Guard against race condition
        if exc.errno != errno.EEXIST:
            exit()
open(TEMP_FILE_LOCATION, 'wb').write(imageResponse.content)
os.system("gsettings set org.gnome.desktop.background picture-uri file://" + TEMP_FILE_LOCATION)
#todo remove file
print("executed command: " + "gsettings set org.gnome.desktop.background picture-uri file://" + TEMP_FILE_LOCATION)