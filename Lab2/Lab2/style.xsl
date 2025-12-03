<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" encoding="UTF-8" indent="yes"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>Газета факультету: <xsl:value-of select="FacultyNewspaper/@name"/></title>
        <style>
          body { font-family: Arial, sans-serif; margin: 20px; }
          table { border-collapse: collapse; width: 100%; }
          th, td { border: 1px solid #ddd; padding: 8px; text-align: left; vertical-align: top; }
          th { background-color: #f2f2f2; }
          h1 { color: #333; }
          h2 { color: #555; }
          .entry { margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; }
          .meta { font-size: 0.9em; color: #777; }
        </style>
      </head>
      <body>
        <h1>Мережна газета: <xsl:value-of select="FacultyNewspaper/@name"/></h1>
        <p><i>(Останнє оновлення: <xsl:value-of select="FacultyNewspaper/@lastUpdated"/>)</i></p>

        <table>
          <tr>
            <th>Тип</th>
            <th>Назва</th>
            <th>Анотація</th>
            <th>Автори</th>
            <th>Відгуки</th>
          </tr>
          
          <xsl:apply-templates select="FacultyNewspaper/Entry"/>
          
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Entry">
    <tr>

      <td>
        <xsl:value-of select="@type"/>
        <xsl:if test="@department">
          <br/>(<xsl:value-of select="@department"/>)
        </xsl:if>
        <xsl:if test="@isFeatured = 'true'">
          <br/><b>[Рекомендовано]</b>
        </xsl:if>
      </td>
      
      <td><b><xsl:value-of select="Title"/></b></td>
      <td><xsl:value-of select="Annotation"/></td>
      
      <td>
        <xsl:for-each select="Author">
          <xsl:value-of select="Name"/>
          <br/>
        </xsl:for-each>
      </td>
      
      <td>
        <xsl:for-each select="Reviews/Review">
          <b><xsl:value-of select="@reader"/></b> (Оцінка: <xsl:value-of select="@score"/>):
          <br/>
          <i>"<xsl:value-of select="."/>"</i>
          <br/>
        </xsl:for-each>
      </td>
      
    </tr>
  </xsl:template>

</xsl:stylesheet>