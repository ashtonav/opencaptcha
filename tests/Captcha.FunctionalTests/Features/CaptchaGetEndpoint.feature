Feature: CaptchaGet
I want to send different captcha requests and assure the image is generated

    Scenario Outline: Send captcha requests
        Given I have a captcha request using get with following parameters:
          | Text   | Width   | Height   | Difficulty   |
          | <Text> | <Width> | <Height> | <Difficulty> |
        When I send the get request to the Create endpoint of the CaptchaController
        Then I expect a captcha image to be returned with the following attributes:
          | Width           | Height           |
          | <ExpectedWidth> | <ExpectedHeight> |
        Then I expect a captcha image to contain at least '<FirstColorPixels>' pixels of color '<FirstColorHex>' and at least '<SecondColorPixels>' pixels of color '<SecondColorHex>'

        Examples:
          | Text         | Width | Height | Difficulty  | ExpectedWidth | ExpectedHeight | FirstColorPixels | FirstColorHex | SecondColorPixels | SecondColorHex |
          | مرحبًا       |       |        |             | 400           | 100            | 2300             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | 你好           |       |        |             | 400           | 100            | 2500             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | こんにちは        |       |        |             | 400           | 100            | 2500             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | 안녕하세요        |       |        |             | 400           | 100            | 3000             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Здравствуйте |       |        |             | 400           | 100            | 3000             | #FFD3D3D3     | 28000             | #FFFFFFFF      |
          | Bonjour      |       |        |             | 400           | 100            | 3000             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Guten Tag    |       |        |             | 400           | 100            | 3000             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Selam        |       |        |             | 400           | 100            | 2500             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Γεια σας     |       |        |             | 400           | 100            | 3000             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Lorem        |       |        |             | 400           | 100            | 2500             | #FFD3D3D3     | 30000             | #FFFFFFFF      |
          | Ipsum        |       | 200    |             | 400           | 200            | 6000             | #FFD3D3D3     | 60000             | #FFFFFFFF      |
          | helloworld   | 200   |        | Easy        | 200           | 100            | 550              | #FFD3D3D3     | 10000             | #FFFFFFFF      |
          | bar          | 300   | 300    | Medium      | 300           | 300            | 4500             | #FFD3D3D3     | 70000             | #FFFFFFFF      |
          | foo          | 400   | 400    | Hard        | 400           | 400            | 6000             | #FFD3D3D3     | 110000            | #FFFFFFFF      |
          | Ciao         | 200   |        | Easy        | 200           | 100            | 850              | #FFD3D3D3     | 10000             | #FFFFFFFF      |
          | Olá          | 300   | 300    | Challenging | 300           | 300            | 6000             | #FFD3D3D3     | 64000             | #FFFFFFFF      |
          | สวัสดี       | 400   | 400    | Hard        | 400           | 400            | 12000            | #FFD3D3D3     | 120000            | #FFFFFFFF      |
