# OpenCaptcha

![banner](docs/logo.png)

[![codecov](https://codecov.io/gh/ashtonav/opencaptcha/graph/badge.svg?token=ZD0L2LC2U0)](https://codecov.io/gh/ashtonav/opencaptcha)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ashtonav_opencaptcha&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=ashtonav_opencaptcha)
[![.NET](https://github.com/ashtonav/opencaptcha/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ashtonav/opencaptcha/actions/workflows/dotnet.yml)
[![license](https://img.shields.io/github/license/ashtonav/opencaptcha.svg)](LICENSE)

OpenCaptcha is an open source, free, public API that generates CAPTCHA images from text.

OpenCaptcha provides:
- A free, public API at [api.opencaptcha.io](https://api.opencaptcha.io).
- Documentation at [opencaptcha.io](https://opencaptcha.io).
- Self-hosting support (see the [Installation](#installation) section).

## Usage

### Example 1: Generated CAPTCHA images are randomly distorted.

![banner](docs/captcha_examples.gif)

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=POST&url=https%3A%2F%2Fapi.opencaptcha.io%2Fcaptcha&bodyMode=raw&contentType=application%2Fjson&rawParams=%7B%22text%22%3A%22captcha%22%7D)

```bash
curl -X 'POST' \
  'https://api.opencaptcha.io/captcha' \
  -H 'Content-Type: application/json' \
  -d '{ "text": "captcha" }'
```

### Example 2: Generated CAPTCHA images can contain text in many languages.

![banner](docs/captcha_example_multilingual.gif)

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=POST&url=https%3A%2F%2Fapi.opencaptcha.io%2Fcaptcha&bodyMode=raw&contentType=application%2Fjson&rawParams=%7B%22text%22%3A%22%E6%99%AE%E9%80%9A%22%7D)

```bash
curl -X 'POST' \
  'https://api.opencaptcha.io/captcha' \
  -H 'Content-Type: application/json' \
  -d '{ "text": "普通" }'
```

## Installation

### Option 1: Using Docker (recommended for self-hosting)

#### Requirements
- Docker
- Windows Operating System

#### How to Run
1. Switch Docker to use Windows containers (see [Docker docs](https://docs.docker.com/desktop/setup/install/windows-install/#:~:text=How%20do%20I%20switch%20between%20Windows%20and%20Linux%20containers%3F)).
2. From the root folder of the project, run the following commands:
   ```bash
   docker build -t opencaptcha -f ./src/Captcha.WebApi/Dockerfile .
   docker run -it -p 5280:8080 opencaptcha
   ```
3. The API can be accessed at [http://localhost:5280](http://localhost:5280).

### Option 2: Using Visual Studio (recommended for development purposes)

#### Requirements
- Visual Studio 2022
    - With ASP.NET and web development installed from the Visual Studio Installer
- .NET 9 SDK
- Windows Operating System

#### How to Run
1. Open the solution in Visual Studio 2022.
2. Build and launch the Captcha.WebApi project.
3. The API can be accessed at [https://localhost:5280](https://localhost:5280).

#### How to Test
1. Open the solution in Visual Studio 2022.
2. Run the tests in Test Explorer.

## Acknowledgments

A significant portion of this project, especially the CAPTCHA generation code, is based on work originally published on February 9, 2004, by [BrainJar](https://www.codeproject.com/Articles/5947/CAPTCHA-Image).

## Contributing

Pull requests are accepted.

## License

[MIT](https://choosealicense.com/licenses/mit/)
