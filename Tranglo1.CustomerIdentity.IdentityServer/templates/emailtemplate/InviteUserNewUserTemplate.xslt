<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
<xsl:template match="Profile">
<html xmlns="http://www.w3.org/1999/xhtml"
      xmlns:v="urn:schemas-microsoft-com:vml"
      xmlns:o="urn:schemas-microsoft-com:office:office">
<head>
    <meta http-equiv="Content-type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width"/>
    <!--[if !mso]><!-->
    <link href="https://fonts.googleapis.com/css?family=Muli:400,400i,700,700i"
          rel="stylesheet" />
    <!--<![endif]-->

    <style type="text/css" media="screen">
        /* Linked Styles */
        body {
            padding: 0 !important;
            margin: 0 !important;
            display: block !important;
            min-width: 100% !important;
            width: 100% !important;
            background: white;
            -webkit-text-size-adjust: none;
        }

        a {
            color: #66c7ff;
            text-decoration: none;
        }

        p {
            padding: 0 !important;
            margin: 0 !important;
        }

        img {
            -ms-interpolation-mode: bicubic; /* Allow smoother rendering of resized image in Internet Explorer */
        }

        .center {
            margin: auto;
            width: 50%;
        }

        .font-style {
            color: #000000;
            font-family: Montserrat, sans-serif;
            font-size: 20px;
            line-height: 46px;
        }

        .mb-1r {
            margin-bottom: 1rem !important;
        }

        .mb-4r {
            margin-bottom: 4rem !important;
        }

        .linkButton {
            background-color: #15bdf0;
            border-radius: 28px;
            border: 1px solid #15bdf0;
            display: inline-block;
            cursor: pointer;
            color: #ffffff;
            font-family: Arial;
            font-size: 17px;
            font-weight: bold;
            padding: 16px 50px;
            text-decoration: none;
        }

        .bg {
            background-color: white;
            width: 100%;
            height: 100%;
            padding: 20px;
        }

        hr.style-six {
            border: 0;
            height: 0;
            border-top: 1px solid rgba(0, 0, 0, 0.1);
            border-bottom: 1px solid rgba(255, 255, 255, 0.3);
        }

        .tradeMark {
            margin-top: 30px;
            margin-bottom: 10px;
            color: #000000;
            font-family: Montserrat, sans-serif;
            font-size: 15px;
        }

        /* Mobile styles */
        @media only screen and (max-device-width: 480px), only screen and (max-width: 480px) {

            .center {
                min-width: 90%;
                margin: auto;
            }

            .linkButton {
                background-color: #15bdf0;
                border-radius: 28px;
                border: 1px solid #15bdf0;
                display: inline-block;
                cursor: pointer;
                color: #ffffff;
                font-family: Arial;
                font-size: 20px;
                font-weight: bold;
                padding: 16px 50px;
                text-decoration: none;
            }

            .font-style {
                color: #000000;
                font-family: Montserrat, sans-serif;
                font-size: 20px;
                line-height: 30px;
            }
        }
    </style>
</head>
<body class="body"
      style="
      padding: 0 !important;
      margin: 0 !important;
      display: block !important;
      min-width: 100% !important;
      width: 100% !important;
      background: rgba(242, 242, 242, 1);
      -webkit-text-size-adjust: none;
    ">

    <div class="center">
        <div class="bg">
            <p class="font-style mb-1r">
                Hi <xsl:value-of select="UserName"/>,
            </p>
            <p class="font-style mb-1r">
                Welcome to Tranglo Connect.
            </p>
            <p class="font-style mb-4r">
                <xsl:value-of select="CurrentUserName"/> has invited you to be part of the <xsl:value-of select="InviterCompanyName"/> team.
                Please proceed to verification and complete to set up your account.
            </p>
            	<xsl:element name="a">
						<xsl:attribute name="target">_blank</xsl:attribute>
                        <xsl:attribute name="class">linkButton mb-1r</xsl:attribute>
						<xsl:attribute name="href">
								<xsl:value-of select="EmailServiceUri"/>
							</xsl:attribute>
              <span style="color:White">Set Up My Account</span>
                    </xsl:element>
            <hr class="style-six" />
            <center class="tradeMark">
                <xsl:value-of select="CurrentYear"/> Tranglo. All rights reserved.
            </center>
        </div>
    </div>



</body>
</html>
	</xsl:template>
</xsl:stylesheet>
