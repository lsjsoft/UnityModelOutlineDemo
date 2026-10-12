# Unity Outline Demo

使用法线直接膨胀画Outline的Pass的效果
![](https://blog.coolcoding.cn/wp-content/uploads/2026/10/image-2.png)

在转角处有明显的裂缝

原因：每个共用顶点，有N个不同的法线


需要将共用顶点的法线合并为一根光滑的法线

方法1：用光滑组
缺点：法线被烘焙法线占用

方法2：平滑法线烘到uv中
不影响原法线，但是需要从uv中重新解出

在UnityModelOutlineDemo示例中，实现了2套烘焙

1套使用8面体压缩到float2的uv6中

1套不压缩直接存储float3到uv7中




