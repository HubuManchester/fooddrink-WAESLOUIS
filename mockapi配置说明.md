# mockapi.io 数据源配置说明

当前项目已经支持从 mockapi.io 读取和新增食品数据。

## 1. 当前数据来源

项目原来使用的是本地 mock 数据，位置在：

```text
FoodDrinkApp/Services/FoodCatalogService.cs
```

现在已改成：

- 优先使用 mockapi.io REST API
- 如果没有配置 API 地址，或网络暂时不可用，则使用本地兜底数据，保证 App 不会崩溃

## 2. 在 mockapi.io 创建数据表

1. 打开 [mockapi.io](https://mockapi.io)
2. 新建或进入一个 Project
3. 点击添加 Resource
4. Resource 名称建议填写：

```text
foods
```

5. 添加以下字段：

| 字段名 | 类型建议 | 说明 |
|---|---|---|
| name | String | 食品或饮品名称 |
| category | String | 分类，例如 早餐、午餐、饮品 |
| description | String | 描述 |
| calories | Number | 热量 |
| protein | Number | 蛋白质 |
| carbs | Number | 碳水 |
| fat | Number | 脂肪 |
| allergyNote | String | 过敏提示 |
| tags | String | 搜索标签 |

`id` 字段由 mockapi.io 自动生成，不需要手动添加。

## 3. 示例数据

可以在 mockapi.io 中添加类似数据：

```json
{
  "name": "Berry Yogurt Bowl",
  "category": "Breakfast",
  "description": "Greek yogurt with mixed berries, oats, and a small drizzle of honey.",
  "calories": 340,
  "protein": 24,
  "carbs": 42,
  "fat": 8,
  "allergyNote": "Contains dairy and gluten.",
  "tags": "healthy breakfast yogurt berries"
}
```

```json
{
  "name": "Chicken Brown Rice Box",
  "category": "Lunch",
  "description": "Grilled chicken breast with brown rice, spinach, cucumber, and lemon dressing.",
  "calories": 520,
  "protein": 38,
  "carbs": 58,
  "fat": 14,
  "allergyNote": "No common allergens recorded.",
  "tags": "meal prep protein lunch"
}
```

```json
{
  "name": "Iced Matcha Latte",
  "category": "Drink",
  "description": "Matcha, milk, and ice. A lower-sugar version is recommended.",
  "calories": 180,
  "protein": 8,
  "carbs": 22,
  "fat": 6,
  "allergyNote": "Contains dairy unless plant-based milk is selected.",
  "tags": "drink caffeine matcha latte"
}
```

## 4. 把 API 地址填入项目

创建 Resource 后，mockapi.io 会生成类似这样的地址：

```text
https://682xxxx.mockapi.io/api/v1/foods
```

打开：

```text
FoodDrinkApp/Services/MockApiConfig.cs
```

把 `EndpointUrl` 改成你的地址：

```csharp
public const string EndpointUrl = "https://682xxxx.mockapi.io/api/v1/foods";
```

重新运行 App 后：

- 首页列表会从 mockapi.io 获取食品数据
- 添加记录页保存时会 POST 到 mockapi.io
- 详情页会通过 API 获取单条数据

## 5. 录屏时如何说明

可以这样讲：

> 项目最开始使用本地 mock 数据。为了更符合真实移动应用的数据来源，我将数据层改成了 mockapi.io REST API。`FoodCatalogService` 使用 HttpClient 获取食品列表、添加新记录和获取详情。如果网络不可用，应用会使用本地兜底数据，避免演示时崩溃。
