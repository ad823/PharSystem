# 人員簽章 API 文檔

## 概述

人員簽章模塊提供了用戶簽章的上傳、查詢和刪除功能。每個用戶只能有一個簽章記錄，新增時自動生成，修改時自動更新。

---

## API 接口清單

| 接口 | 方法 | 說明 |
|------|------|------|
| `/api/personSignature/init` | POST | 初始化簽章數據表 |
| `/api/personSignature/personSignature_by_guid` | POST | 獲取指定人員的簽章 |
| `/api/personSignature/personSignature_by_guids` | POST | 批量獲取多個人員的簽章（PDF預覽用） |
| `/api/personSignature/add_PersonSignature` | POST | 新增或更新人員簽章 |
| `/api/personSignature/delete_PersonSignature` | POST | 刪除人員簽章（清空Base64） |

---

## 詳細接口說明

### 1. 初始化簽章數據表
**路由**: `POST /api/personSignature/init`

**說明**: 初始化 `person_Signature` 數據表，包括字段檢查和自動建表

**請求體**:
```json
{
  "ServerName": "Main",           // 可選，默認為Main
  "ServerType": "網頁"             // 可選，默認為網頁
}
```

**成功回應 (200)**:
```json
{
  "Code": 200,
  "Result": "初始化成功",
  "Data": {
    "TableName": "person_Signature",
    "Columns": [...]
  }
}
```

**失敗回應 (-200)**:
```json
{
  "Code": -200,
  "Result": "找無Server資料!"
}
```

---

### 2. 獲取指定人員的簽章
**路由**: `POST /api/personSignature/personSignature_by_guid`

**說明**: 根據 PersonGUID 獲取該人員的簽章記錄

**請求體**:
```json
{
  "Value": "550e8400-e29b-41d4-a716-446655440000"  // PersonGUID，必填
}
```

**成功回應 (200)**:
```json
{
  "Code": 200,
  "Result": "獲取簽章成功",
  "Data": {
    "GUID": "550e8400-e29b-41d4-a716-446655440001",
    "PersonGUID": "550e8400-e29b-41d4-a716-446655440000",
    "SignatureBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
    "ContentType": "image/png",
    "FileSize": "15234",
    "CreatBy": "A001",
    "CreatAt": "2026-05-19T10:30:00",
    "UpdateAt": "2026-05-19T14:45:30"
  },
  "TimeTaken": "12ms"
}
```

**失敗回應 (-200)**:
```json
{
  "Code": -200,
  "Result": "查無該用戶簽章資料"
}
```

---

### 3. 批量獲取多個人員的簽章（PDF預覽用）
**路由**: `POST /api/personSignature/personSignature_by_guids`

**說明**: 根據多個 PersonGUID 批量獲取簽章，用於網頁預覽或生成 PDF

**請求體**:
```json
{
  "ValueAry": [
    "550e8400-e29b-41d4-a716-446655440000",
    "550e8400-e29b-41d4-a716-446655440001",
    "550e8400-e29b-41d4-a716-446655440002"
  ]
}
```

**成功回應 (200)**:
```json
{
  "Code": 200,
  "Result": "獲取3筆簽章成功",
  "Data": [
    {
      "GUID": "550e8400-e29b-41d4-a716-446655440001",
      "PersonGUID": "550e8400-e29b-41d4-a716-446655440000",
      "SignatureBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
      "ContentType": "image/png",
      "FileSize": "15234",
      "CreatBy": "A001",
      "CreatAt": "2026-05-19T10:30:00",
      "UpdateAt": "2026-05-19T14:45:30"
    },
    {
      "GUID": "550e8400-e29b-41d4-a716-446655440002",
      "PersonGUID": "550e8400-e29b-41d4-a716-446655440001",
      "SignatureBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
      "ContentType": "image/jpeg",
      "FileSize": "18234",
      "CreatBy": "A002",
      "CreatAt": "2026-05-18T09:15:00",
      "UpdateAt": "2026-05-19T11:22:00"
    }
  ],
  "TimeTaken": "25ms"
}
```

**失敗回應 (-200)**:
```json
{
  "Code": -200,
  "Result": "PersonGUID陣列不可為空"
}
```

---

### 4. 新增或更新人員簽章
**路由**: `POST /api/personSignature/add_PersonSignature`

**說明**: 上傳簽章圖片，自動判斷新增或更新
- 若該 PersonGUID 尚無簽章 → 新增記錄
- 若該 PersonGUID 已有簽章 → 更新簽章內容
- 自動從 person_page 表取得用戶 ID 作為 CreatBy

**請求格式**: `multipart/form-data`

**表單參數**:
| 參數名 | 類型 | 必填 | 說明 |
|--------|------|------|------|
| PersonGUID | string | ✓ | 人員GUID |
| file | file | ✓ | 簽章圖片文件 |

**文件限制**:
- **格式**: jpg, jpeg, png, gif, webp
- **大小**: 最多 2MB
- **ContentType**: image/jpeg, image/png, image/gif, image/webp

**JavaScript 上傳示例**:
```javascript
const formData = new FormData();
formData.append('PersonGUID', '550e8400-e29b-41d4-a716-446655440000');
formData.append('file', fileInput.files[0]);  // HTML file input

fetch('/api/personSignature/add_PersonSignature', {
  method: 'POST',
  body: formData
})
.then(response => response.json())
.then(data => {
  if (data.Code === 200) {
    console.log('簽章上傳成功', data.Data);
  } else {
    console.error('錯誤:', data.Result);
  }
});
```

**成功回應 (200) - 新增時**:
```json
{
  "Code": 200,
  "Result": "新增簽章成功",
  "Data": {
    "GUID": "550e8400-e29b-41d4-a716-446655440001",
    "PersonGUID": "550e8400-e29b-41d4-a716-446655440000",
    "SignatureBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
    "ContentType": "image/png",
    "FileSize": "15234",
    "CreatBy": "A001",
    "CreatAt": "2026-05-19T10:30:00",
    "UpdateAt": "2026-05-19T10:30:00"
  },
  "TimeTaken": "145ms"
}
```

**成功回應 (200) - 更新時**:
```json
{
  "Code": 200,
  "Result": "更新簽章成功",
  "Data": {
    "GUID": "550e8400-e29b-41d4-a716-446655440001",
    "PersonGUID": "550e8400-e29b-41d4-a716-446655440000",
    "SignatureBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUA...",
    "ContentType": "image/png",
    "FileSize": "15234",
    "CreatBy": "A001",
    "CreatAt": "2026-05-19T10:30:00",
    "UpdateAt": "2026-05-19T14:45:30"
  },
  "TimeTaken": "145ms"
}
```

**失敗回應 (-200) 示例**:

文件大小超限:
```json
{
  "Code": -200,
  "Result": "文件大小超過限制（最大2MB，當前2.5MB）"
}
```

文件格式不符:
```json
{
  "Code": -200,
  "Result": "只接受圖片格式（jpg, png, gif, webp）"
}
```

找不到用戶:
```json
{
  "Code": -200,
  "Result": "找無該人員資料"
}
```

---

### 5. 刪除人員簽章
**路由**: `POST /api/personSignature/delete_PersonSignature`

**說明**: 清空簽章的 SignatureBase64 欄位（保留記錄，不刪除）

**請求體**:
```json
{
  "Value": "550e8400-e29b-41d4-a716-446655440000"  // PersonGUID 或 GUID
}
```

**成功回應 (200)**:
```json
{
  "Code": 200,
  "Result": "刪除簽章成功",
  "TimeTaken": "15ms"
}
```

**失敗回應 (-200)**:
```json
{
  "Code": -200,
  "Result": "查無簽章資料"
}
```

**參數說明**:
| 參數 | 類型 | 說明 |
|------|------|------|
| Value | string | PersonGUID 或簽章 GUID，二者均可 |

---

## 數據表結構

### person_Signature 表

| 欄位名 | 類型 | 長度 | 索引 | 說明 |
|--------|------|------|------|------|
| GUID | VARCHAR | 40 | PRIMARY | 簽章唯一識別碼 |
| PersonGUID | VARCHAR | 40 | INDEX | 關聯person_page.GUID |
| SignatureBase64 | LONGTEXT | - | - | Base64編碼的簽章圖片 |
| ContentType | VARCHAR | 50 | - | 文件MIME類型 (image/png等) |
| FileSize | VARCHAR | 50 | - | 文件大小（字節） |
| CreatBy | VARCHAR | 40 | - | 創建者用戶ID（自動從person_page取得） |
| CreatAt | DATETIME | - | - | 創建時間 |
| UpdateAt | DATETIME | - | - | 最後更新時間 |

---

## 業務規則

1. **一人一簽章**: 每個用戶只能有一個簽章記錄
2. **自動Upsert**: `add_PersonSignature` 接口自動判斷新增或更新
3. **軟刪除**: `delete_PersonSignature` 接口只清空簽章內容，不刪除記錄
4. **時間戳**: 
   - CreatAt：首次上傳時自動生成，之後不更改
   - UpdateAt：每次修改時更新為當前時間
5. **文件限制**:
   - 格式：jpg, jpeg, png, gif, webp
   - 大小：最多 2MB
6. **用戶驗證**: 上傳前自動驗證 PersonGUID 是否存在於 person_page 表，並自動取得用戶 ID
7. **CreatBy 自動賦值**: 不需要前端傳入，系統自動從 person_page 表查詢並填入

---

## 錯誤碼

| 錯誤碼 | 說明 |
|--------|------|
| 200 | 操作成功 |
| -200 | 通用錯誤（具體信息見 Result） |

**常見錯誤信息**:
- `PersonGUID不可為空` - PersonGUID未提供或為空
- `未提供有效的圖片文件` - 未上傳文件或文件為空
- `文件大小超過限制（最大2MB）` - 文件超過2MB
- `只接受圖片格式（jpg, png, gif, webp）` - 文件格式不符合要求
- `找無該人員資料` - PersonGUID在person_page表中不存在
- `查無該用戶簽章資料` - 該用戶尚無簽章記錄
- `查無簽章資料` - 刪除時找不到簽章記錄
- `PersonGUID陣列不可為空` - 批量查詢時未提供 GUID 數組
- `找無Server資料!` - 服務器配置未找到

---

## 前端集成建議

### 簽章上傳表單
```html
<form id="signatureForm">
  <input type="hidden" id="personGUID" value="">
  
  <div>
    <label>選擇簽章圖片：</label>
    <input type="file" id="signatureFile" accept="image/*">
  </div>
  
  <button type="submit">上傳簽章</button>
</form>

<script>
document.getElementById('signatureForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  
  const file = document.getElementById('signatureFile').files[0];
  if (!file) {
    alert('請選擇圖片');
    return;
  }
  
  const formData = new FormData();
  formData.append('PersonGUID', document.getElementById('personGUID').value);
  formData.append('file', file);
  
  const response = await fetch('/api/personSignature/add_PersonSignature', {
    method: 'POST',
    body: formData
  });
  
  const result = await response.json();
  if (result.Code === 200) {
    alert('簽章上傳成功');
    displaySignature(result.Data.SignatureBase64, result.Data.ContentType);
  } else {
    alert('錯誤: ' + result.Result);
  }
});

function displaySignature(base64, contentType) {
  const img = document.getElementById('signaturePreview');
  img.src = `data:${contentType};base64,${base64}`;
}
</script>
```

### 單個簽章查詢和顯示
```html
<div id="signatureContainer">
  <img id="signaturePreview" style="display:none;max-width:200px;">
  <p id="signatureStatus"></p>
</div>

<script>
async function loadSignature(personGUID) {
  const response = await fetch('/api/personSignature/personSignature_by_guid', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      Value: personGUID
    })
  });
  
  const result = await response.json();
  const statusEl = document.getElementById('signatureStatus');
  
  if (result.Code === 200) {
    const img = document.getElementById('signaturePreview');
    img.src = `data:${result.Data.ContentType};base64,${result.Data.SignatureBase64}`;
    img.style.display = 'block';
    statusEl.textContent = `簽章日期: ${result.Data.CreatAt}`;
  } else {
    statusEl.textContent = result.Result;
  }
}
</script>
```

### 批量簽章查詢（PDF預覽）
```html
<div id="signaturesContainer"></div>

<script>
async function loadSignatures(personGUIDs) {
  const response = await fetch('/api/personSignature/personSignature_by_guids', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      ValueAry: personGUIDs
    })
  });
  
  const result = await response.json();
  const container = document.getElementById('signaturesContainer');
  
  if (result.Code === 200) {
    result.Data.forEach(sig => {
      const div = document.createElement('div');
      const img = document.createElement('img');
      img.src = `data:${sig.ContentType};base64,${sig.SignatureBase64}`;
      img.style.maxWidth = '150px';
      
      const info = document.createElement('p');
      info.textContent = `GUID: ${sig.PersonGUID}`;
      
      div.appendChild(img);
      div.appendChild(info);
      container.appendChild(div);
    });
  } else {
    container.textContent = result.Result;
  }
}

// 使用範例
loadSignatures([
  'a9c4e6f2-3b71-4d8a-9f25-6e0b7c13d4a8',
  '9a59f677-75fb-4e2b-979c-ca9d282e5492'
]);
</script>
```

---

## 版本歷史

| 版本 | 日期 | 說明 |
|------|------|------|
| 1.2 | 2026-05-19 | 完整功能實現，支持批量查詢（PDF預覽用） |
| 1.1 | 2026-05-19 | 移除UserID參數，自動從person_page表取得用戶ID |
| 1.0 | 2026-05-19 | 初版本，支持新增/更新/查詢/刪除 |
