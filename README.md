# typora-uploader



## smms-uploader

The Lightweight SM.MS Image Uploader of Typora Editor for .NET Core.

---

### Install

[https://www.nuget.org/packages/smms-uploader](https://www.nuget.org/packages/smms-uploader)


```bash
dotnet tool install -g smms-uploader
```

---

### Setting

**English Language**

![en-US.png](https://raw.githubusercontent.com/Run2948/typora-uploader/master/docs/en-US.png)

---

**Chinese Language**

![zh-CN.png](https://raw.githubusercontent.com/Run2948/typora-uploader/master/docs/zh-CN.png)

---
### Usage

* Directly paste (Ctrl+V) from the clipboard to upload in the Typora editor.
* Automatically trigger upload after selecting a picture in the Typora editor.



## minio-uploader

The Lightweight MinIO Image Uploader of Typora Editor for .NET Core.

---

### Install

[https://www.nuget.org/packages/minio-uploader](https://www.nuget.org/packages/minio-uploader)


```bash
dotnet tool install -g minio-uploader
```

---

### Setting

**Configuration File**

File name: `conf.uploader.json`

Open the path:  `explorer "%AppData%\Typora\conf"`

```json
{
  "minio-uploader": {
    "endpoint": "127.0.0.1:9000",
    "accessKey": "minioadmin",
    "secretKey": "minioadmin",
    "withSSL": false,
    "bucketName": "images",
    /* {filename} 会替换成原文件名, 配置这项需要注意路径中的中文会被过滤掉以防止出现乱码 */
    /* {rand:6} 会替换成随机数,后面的数字是随机数的位数 */
    /* {time} 会替换成时间戳 */
    /* {yyyy} 会替换成四位年份 */
    /* {yy} 会替换成两位年份 */
    /* {mm} 会替换成两位月份 */
    /* {dd} 会替换成两位日期 */
    /* {hh} 会替换成两位小时 */
    /* {ii} 会替换成两位分钟 */
    /* {ss} 会替换成两位秒 */      
    "pathFormat": "{yyyy}/{mm}/{dd}/{time}{rand:6}"
  }
}
```

**English Language**

![en-US.png](https://raw.githubusercontent.com/Run2948/typor-uploader/master/docs/en-US-2.png)

---

**Chinese Language**

![zh-CN.png](https://raw.githubusercontent.com/Run2948/typora-uploader/master/docs/zh-CN-2.png)

---
### Usage

* Directly paste (Ctrl+V) from the clipboard to upload in the Typora editor.
* Automatically trigger upload after selecting a picture in the Typora editor.

