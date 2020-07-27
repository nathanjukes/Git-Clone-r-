<h1 align="center">
  Git-Clone(r)
</h1>

<h4 align="center">A C# CLI driven Github Cloner with extended features, commonly used for backing up repos.
</h4>

<p align="center">
  <a href="https://scrutinizer-ci.com/g/pH7Software/pH7-Social-Dating-CMS/build-status/master">
      <img src="https://scrutinizer-ci.com/g/pH7Software/pH7-Social-Dating-CMS/badges/build.png?b=master">
  </a>
  <a href="https://img.shields.io/badge/version-v1.0-blue">
    <img src="https://img.shields.io/badge/version-v1.0-blue">
  </a>
  <a href="https://github.com/nathanjukes/Git-Clone-r-/blob/master/LICENSE.md">
    <img src="https://img.shields.io/github/license/Naereen/StrapDown.js.svg">
  </a>
  <a href="https://twitter.com/intent/tweet?url=https%3A%2F%2Fgithub.com%2Fnathanjukes%2FGit-Clone--r-&text=Check%20out%20this%20Github%20Clone%20Tool%20on%20Github:">
    <img src="https://img.shields.io/twitter/url/http/shields.io.svg?style=social">
  </a>
</p>

## Installation
Head to this directory to find the application: 
```bash
"C:\Git-Clone-r-\Application" 
```
Or download here:
[(64-bit)](https://github.com/nathanjukes/Git-Clone-r-/blob/master/Git-Clone(r)/Git-Clone(r)/bin/Release/Application.zip) and Run the `Git-Clone(r).exe` after configuring the `UserSettings.json`


![Homescreen Image](https://github.com/nathanjukes/Git-Clone-r-/blob/master/Assets/Homescreen.JPG)



## Change Log

- 1.00 - Released XX/XX/XX


## Configuration
To use this App you must first edit the `Secrets.json` file and insert the following data:
```bash
{
    "clientID": "YourClientID",
    "clientSecret": "YourClientSecret"
}   
```
To get your ClientID and ClientSecret you must first login to the [Github Developers Page](https://github.com/settings/developers). From here, head to the `OAuth Apps` page and hit `New OAuth App`. You can name it what you want but you must set the `Homepage URL` and `Authorization callback URL` `http://localhost:29522/`. Finally, retrieve the `Client ID` and `Client Secret` values on the homepage of the App.

#### Please Note:
It's unfortunate that you must retrieve those values manually but at the time of creation, I did not have the server capacity to host the Backend 24/7 and I could not ensure it would be available at all times.


### License
[MIT](https://github.com/nathanjukes/Git-Clone-r-/blob/master/LICENSE.md)
