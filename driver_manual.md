# MCUSB4sd 运动控制板说明书

## 目录
- [第1章 概述与特点](#第1章-概述与特点)
- [第2章 功能](#第2章-功能)
- [第6章 API函数参考](#第6章-api函数参考)

---

# 第1章 概述与特点

## 1-1 概述

MCUSB4sd 是一款符合 USB 全速（相当于 V1.1）规范的 4 轴运动控制板。  
配合脉冲列输入方式的位置控制型电机驱动器使用，可最多控制 4 台电机。  
所控制的 4 个轴可以相互独立地运动。  

本手册介绍了 MCUSB4sd 的功能，以及为了便于控制 MCUSB4sd，在 Windows 系统中安装驱动程序、动态链接库和导入库的方法，并详细说明了各控制函数的用法。

## 1-2 特点

- **最高输出频率** 8.191 Mpps  
- **直线／S 曲线加减速功能**（支持非对称）  
- **三角驱动回避功能**  
- **支持多种脉冲输出** 1 脉冲／2 脉冲／2 相脉冲输出  
- **多种驱动模式** 定位／连续／信号检测驱动功能  
- **实时调整功能** 速度／移动量覆写（Override）功能  
- **限位保护** ＋／－限位停止功能（可选择急停或减速停止）  
- **伺服接口** 与伺服电机驱动器的接口功能  
- **丰富的I/O** 通用输入信号 16 点（每轴 4 点）、通用输出信号 12 点（每轴 3 点）  
- **脉冲计数** 输出脉冲与反馈脉冲计数功能  
- **计数器功能** 上述计数器的比较功能与预分频（Prescale）功能  
- **编码器输入** 反馈脉冲计数器可输入 UP/DOWN 信号或 2 相信号（1/2/4 倍频）  
- **中断功能** 驱动结束时及比较结果触发中断功能  
- **同步控制** 多轴同步启动功能  
  - 通过选配线缆可实现多块板间的同步启动  

---

# 第2章 功能

## 2-1 驱动类型

MCUSB4sd 提供以下 4 种驱动方式：

### 2-1-1 INDEX PULSE DRIVE（索引脉冲驱动）

**定位驱动模式**，按照指定脉冲数自动停止。
- **最大脉冲数**：16,777,215（0xFFFFFF）
- **驱动类型**：由"LOW SPEED DATA"与"HIGH SPEED DATA"的大小关系决定（见注1）
- **运行中调整**：可修改指定脉冲数与输出频率（见注2）

### 2-1-2 SCAN DRIVE（扫描驱动）

**连续驱动模式**，直到接收到停止指令才会停止。
- **驱动类型**：由"LOW SPEED DATA"与"HIGH SPEED DATA"的大小关系决定（见注1）
- **运行中调整**：可修改输出频率（见注3）

### 2-1-3 SIGNAL SCAN‑A DRIVE（信号扫描‑A 驱动）

**信号检测驱动**，适用于原点检测等场景。
- **停止条件**：在指定信号的指定边沿被检测到时停止
- **加减速模式**：检测到边沿后减速至 LOW SPEED 并停止
- **定速模式**：检测到边沿后立即停止

### 2-1-4 SIGNAL SCAN‑B DRIVE（信号扫描‑B 驱动）

**低速信号检测**，同样用于原点检测等。
- **速度特点**：始终以 LOW SPEED DATA 的速度进行定速驱动
- **停止方式**：检测到指定信号的指定边沿后立即停止

---

### 📋 驱动模式判定规则

| HIGH SPEED 与 LOW SPEED 关系 | 驱动模式 | 说明 |
|---|---|---|
| HIGH SPEED > LOW SPEED | **加减速驱动** | 标准的梯形或S曲线加减速 |
| HIGH SPEED = LOW SPEED | **定速驱动** | 恒定速度运行 |
| HIGH SPEED < LOW SPEED | **数据错误** | 不执行驱动 |

### ⚠️ 运行中调整（Override）限制

#### INDEX PULSE DRIVE 执行中的限制

| 加减速模式 | 脉冲数调整 | 速度调整 | 备注 |
|---|---|---|---|
| **直线加减速** | ✅ 随时可调 | ✅ 随时可调 | 无限制 |
| **S 曲线加减速** | ⚠️ 仅定速阶段 | ⚠️ 调整后无法保证正常停止 | 加减速阶段调整有风险 |
| **非对称模式** | ❌ 无法保证正常停止 | ❌ 无法保证正常停止 | 不推荐运行中调整 |

#### SCAN DRIVE 执行中的限制

| 加减速模式 | 速度调整 |
|---|---|
| **所有模式** | ✅ 随时可调 |

## 2-2 速度设定功能

MCUSB4sd 采用三级速度数据结构：

### 速度计算公式

```
输出频率设置单元 (F_UNIT)     = 1000 / RANGE DATA        [PPS]
低速部频率 (F_LOW)           = F_UNIT × LOW SPEED DATA   [PPS]  
高速部频率 (F_HIGH)          = F_UNIT × HIGH SPEED DATA  [PPS]
当前输出频率 (F_NOW)         = F_UNIT × NOW SPEED DATA   [PPS]
```

### 参数设定范围

| 参数 | 描述 | 设定范围 | 十六进制范围 |
|---|---|---|---|
| **RANGE DATA** | 倍率数据 | 1～8,191 | 0x0001～0x1FFF |
| **LOW SPEED DATA** | 低速部速度数据 | 1～8,191 | 0x0001～0x1FFF |
| **HIGH SPEED DATA** | 高速部速度数据 | 1～8,191 | 0x0001～0x1FFF |

### 速度设置示例

| RANGE DATA | F_UNIT [PPS] | 输出频率范围 [PPS] |
|---|---|---|
| 1 | 1000 | 1,000.0 ～ 8,191,000.0 |
| 10 | 100 | 100.0 ～ 819,100.0 |
| 100 | 10 | 10.0 ～ 81,910.0 |
| 1000 | 1 | 1.0 ～ 8,191.0 |
| 5000 | 0.2 | 0.2 ～ 1,638.2 |

### ⚠️ 重要注意事项

- 所有速度参数均可在驱动停止或运行中随时重写
- 通常通过重写 "HIGH SPEED DATA" 来实现运行中速度调整
- ⚠️ **INDEX PULSE DRIVE 执行中更改 "LOW SPEED DATA" 或 "RANGE DATA" 将无法保证正常的自动减速停止**

## 2-3 加减速模式选择功能

MCUSB4sd 具有 4 种加减速模式：
- 直线加减速模式
- 非对称直线加减速模式  
- S字加减速模式
- 非对称S字加减速模式

**默认模式**：复位后为直线加减速模式

## 2-3-1 直线加减速模式

**最基本的加减速方法**，采用等加速度的直线加减速或2段阶跃状线加减速。

### 参数设置

| 参数 | 描述 | 设定范围 |
|---|---|---|
| **RATE-A, B, C DATA** | 加减速率数据 | 1～8,191（0001H～1FFFH）|
| **RATE CHANGE POINT A-B** | RATE-A到B的变曲点速度 | 0～8,191（0000H～1FFFH）|
| **RATE CHANGE POINT B-C** | RATE-B到C的变曲点速度 | 0～8,191（0000H～1FFFH）|

### 速度曲线图

```
速度
 ↑
 │
HIGH SPEED ┤- - - - - ┌─────────────────────┐
           │          │                     │
RATE CHANGE┤- - - - - ┤←─RATE-C DATA 使用区间─→│
POINT B-C  │        / │                     │ \
           │       /  │                     │  \
RATE CHANGE┤- - - /- - ┤←──RATE-B DATA 使用区间──→┤- - \
POINT A-B  │    /     │                     │     \
           │   /      │                     │      \
LOW SPEED  ┤- /- - - - ┤←────RATE-A DATA 使用区间────→┤- - - \
           │/         │                     │        \
           └──────────┼─────────────────────┼─────────→ 时间
```

### 加减速率使用规则

- **RATE-A DATA 使用区间**：当前输出速度 ≤ RATE CHANGE POINT A-B
- **RATE-B DATA 使用区间**：RATE CHANGE POINT A-B < 当前输出速度 ≤ RATE CHANGE POINT B-C  
- **RATE-C DATA 使用区间**：RATE CHANGE POINT B-C < 当前输出速度

### 简化模式（基本梯形驱动）

当不需要2段阶跃时，RATE CHANGE POINT A-B、B-C 保持默认值 8,191（1FFFH），仅使用 RATE-A DATA：

```
速度
 ↑
 │
HIGH SPEED ┤- - - ┌──────────────────────┐
           │     /│                      │\
           │    / │                      │ \
           │   /  │                      │  \
           │  /   │                      │   \
LOW SPEED  ┤-/- - ┤←─RATE-A DATA 使用区间─→┤- - \
           │/     │                      │      \
           └──────┼──────────────────────┼───────────→ 时间
```

## 2-3-2 非对称直线加减速模式

**加速与减速使用不同参数**的直线加减速模式。

### 参数设置

| 参数 | 描述 | 设定范围 |
|---|---|---|
| **RATE-A DATA** | 加速率数据 | 1～8,191（0001H～1FFFH）|
| **RATE-B DATA** | 减速率数据 | 1～8,191（0001H～1FFFH）|

### 速度曲线图

```
速度
 ↑
 │
HIGH SPEED ┤- - -┌─────────────────────┐
           │    /│                     │\
           │   / │  RATE-B DATA 使用区间 ──→│ \
           │  /  │                     │  \
LOW SPEED  ┤-/- -┤                     │- -\─→ 时间
           │/  ←─── RATE-A DATA 使用区间    │   \
           └─────┼─────────────────────┼────\
```

### ⚠️ 重要限制

由于加速和减速非对称，**INDEX PULSE DRIVE 时无法保证正常的自动减速停止**，因此：
- 减速开始点检出方式采用**「残脉冲数指定方式」**
- 不推荐在运行中调整参数

## 2-3-3 S字加减速模式

**平滑的S曲线加减速**，通过使加速度时间变化，有效抑制对机械系统的冲击。

### 参数设置

| 参数 | 描述 | 设定范围 |
|---|---|---|
| **RATE-A DATA** | 最大倾斜部加减速率数据 | 1～8,191（0001H～1FFFH）|
| **SCW-A DATA** | S字区间速度数据 | 1～4,095（0001H～0FFFH）|

### 速度计算

- **SP1** = LOW SPEED + SCW-A  
- **SP2** = HIGH SPEED - SCW-A

### 速度曲线图

```
速度
 ↑
 │                                            ┌─ SCW-A
HIGH SPEED ┤- - - - -┌─────────────────────┐   │
           │        /│                     │\  │
SP2        ┤- - - - / │                     │ \ ↓
           │       /  │←─RATE-A DATA 使用区间─→│  \
SP1        ┤- - - /- -│                     │- -\ ┌─ SCW-A
           │    /     │                     │    \│
LOW SPEED  ┤- -/- - - │                     │- - -\─→ 时间
           │ /        │                     │      \
           └/─────────┼─────────────────────┼───────\
```

### ⚠️ 调整限制

- 参数可在驱动停止中/执行中随时改写
- **驱动执行中变更无法保证正常的加减速动作**

---

# 第6章 API函数参考

## 6-13 动作控制

### 6-13-1 McsdDriveStart

**启动驱动运动**

#### 函数原型

**C语言**
```c
DWORD McsdDriveStart(HANDLE hDevice, WORD wAxis, WORD wDrive, DWORD dwPulse);
```

**Visual Basic**
```vb
Function McsdDriveStart(ByVal hDevice As Long, ByVal wAxis As Integer,
                       ByVal wDrive As Integer, ByVal dwPulse As Long) As Long
```

#### 参数说明

| 参数 | 描述 |
|---|---|
| `hDevice` | 板卡的设备句柄 |
| `wAxis` | 目标控制轴 |
| `wDrive` | 驱动类型选择 |
| `dwPulse` | 脉冲数（仅INDEX PULSE DRIVE有效）|

#### 驱动类型 (wDrive)

| 值 | 驱动类型 | 说明 |
|---|---|---|
| 0 | + INDEX PULSE DRIVE | 正向定位驱动 |
| 1 | - INDEX PULSE DRIVE | 负向定位驱动 |
| 2 | + SCAN DRIVE | 正向连续驱动 |
| 3 | - SCAN DRIVE | 负向连续驱动 |

#### 脉冲数设定范围 (dwPulse)

- **设定范围**：0～16,777,215（000000H～FFFFFFH）
- **注意**：SCAN DRIVE 模式下此参数被忽略

#### 返回值

| 值 | 说明 |
|---|---|
| 0 | 正常结束 |
| 其他 | 错误代码（参见第7章）|

#### 使用说明

- 可随时调用，但**驱动执行中（BUSY=H）时调用无效**
- 支持正负双向运动控制
- INDEX PULSE DRIVE：到达指定脉冲数后自动停止
- SCAN DRIVE：持续运行直到收到停止命令

### 6-13-2 McsdDriveStop

**停止正在执行的驱动**

#### 函数原型

**C语言**
```c
DWORD McsdDriveStop(HANDLE hDevice, WORD wAxis, WORD wStop);
```

**Visual Basic**
```vb
Function McsdDriveStop(ByVal hDevice As Long, ByVal wAxis As Integer,
                      ByVal wStop As Integer) As Long
```

#### 参数说明

| 参数 | 描述 |
|---|---|
| `hDevice` | 板卡的设备句柄 |
| `wAxis` | 目标控制轴 |
| `wStop` | 停止方式选择 |

#### 停止方式 (wStop)

| 值 | 停止方式 | 状态位 | 说明 |
|---|---|---|---|
| 0 | 减速停止 | SSCED Bit = 1 | 按减速曲线平滑停止 |
| 1 | 紧急停止 | ESCED Bit = 1 | 立即急停，忽略减速过程 |

#### 返回值

| 值 | 说明 |
|---|---|
| 0 | 正常结束 |
| 其他 | 错误代码（参见第7章）|

#### 使用说明

- **可随时执行**，无BUSY状态限制
- 停止状态可通过 `McsdGetStatus` 函数获取
- 减速停止：保护机械系统，平滑停止
- 紧急停止：用于紧急情况或安全考虑

## 6-15 运动参数覆盖

### 6-15-1 McsdHiSpeedOverride

**运行中修改高速数据**

#### 函数原型

**C语言**
```c
DWORD McsdHiSpeedOverride(HANDLE hDevice, WORD wAxis, WORD wKind, double dSpeed);
```

**Visual Basic**
```vb
Function McsdHiSpeedOverride(ByVal hDevice As Long, ByVal wAxis As Integer,
                            ByVal wKind As Integer, ByVal dSpeed As Double) As Long
```

#### 参数说明

| 参数 | 描述 |
|---|---|
| `hDevice` | 板卡的设备句柄 |
| `wAxis` | 目标控制轴 |
| `wKind` | 数据格式选择 |
| `dSpeed` | 速度数据 |

#### 数据格式 (wKind)

| 值 | 格式 | 设定范围 | 说明 |
|---|---|---|---|
| 0 | HI SPEED DATA | 1～8,191 | 内部速度数据格式 |
| 1 | Pulse/Sec | 1～8,191,000 | 直接频率设定 |

#### 使用说明

- **随时可执行**，驱动中会按加减速率调整到新速度
- 设定值小于LOW SPEED时产生数据错误并急停
- 速度上限受初始设定限制
- 推荐用于实时速度调整

### 6-15-2 McsdIndexOverride

**运行中修改目标脉冲数**

#### 函数原型

**C语言**
```c
DWORD McsdIndexOverride(HANDLE hDevice, WORD wAxis, DWORD dwData);
```

**Visual Basic**
```vb
Function McsdIndexOverride(ByVal hDevice As Long, ByVal wAxis As Integer,
                          ByVal dwData As Long) As Long
```

#### 参数说明

| 参数 | 描述 |
|---|---|
| `hDevice` | 板卡的设备句柄 |
| `wAxis` | 目标控制轴 |
| `dwData` | 新的目标脉冲数（0～FFFFFFFh）|

#### 行为模式

根据设定值与当前状态的关系，系统会有不同的响应：

| 情况 | 系统响应 |
|---|---|
| **新目标 ≤ 已输出脉冲数** | 立即停止（防止超程）|
| **新目标过大，无法减速** | 立即开始减速，到达目标停止 |
| **减速中且新目标更大** | 重新加速至HIGH SPEED |

#### 使用限制

- **仅在INDEX PULSE DRIVE执行中有效**
- 其他驱动模式下调用无意义
- 需要考虑当前位置和减速距离

## 6-16 底层硬件访问

### 6-16-1 McsdInP

**读取端口数据**

#### 函数原型

**C语言**
```c
DWORD McsdInP(HANDLE hDevice, WORD wPort, PBYTE pbData);
```

**Visual Basic**
```vb
Function McsdInP(ByVal hDevice As Long, ByVal wPort As Integer,
                ByRef pbData As Byte) As Long
```

### 6-16-2 McsdOutP

**写入端口数据**

#### 函数原型

**C语言**
```c
DWORD McsdOutP(HANDLE hDevice, WORD wPort, BYTE bData);
```

**Visual Basic**
```vb
Function McsdOutP(ByVal hDevice As Long, ByVal wPort As Integer,
                 ByVal bData As Byte) As Long
```

#### ⚠️ 数据一致性警告

使用此函数可能导致以下函数返回值不一致：
- `McsdGetPulseMode`
- `McsdGetLimit`  
- `McsdGetExternalCounterMode`
- `McsdGetExternalCounterClear`
- `McsdGetSignalStop`
- `McsdGetSpeed`

### 6-16-3 McsdDataRead

**命令方式读取数据**

#### 函数原型

**C语言**
```c
DWORD McsdDataRead(HANDLE hDevice, WORD wAxis, WORD wCommand, PDWORD pdwData);
```

**Visual Basic**
```vb
Function McsdDataRead(ByVal hDevice As Long, ByVal wAxis As Integer,
                     ByVal wCommand As Integer, ByRef pdwData As Long) As Long
```

### 6-16-4 McsdDataWrite

**命令方式写入数据**

#### 函数原型

**C语言**
```c
DWORD McsdDataWrite(HANDLE hDevice, WORD wAxis, WORD wCommand, DWORD dwData);
```

**Visual Basic**
```vb
Function McsdDataWrite(ByVal hDevice As Long, ByVal wAxis As Integer,
                      ByVal wCommand As Integer, ByVal dwData As Long) As Long
```

#### ⚠️ 数据一致性警告

使用此函数可能导致以下函数返回值不一致：
- `McsdGetExternalCounterClear`
- `McsdGetSignalStop`
- `McsdGetSpeed`

## 6-17 批量执行优化

### 6-17-2 McsdStartBuffer

**设定函数批量执行**

#### 函数原型

**C语言**
```c
DWORD McsdStartBuffer(HANDLE hDevice, WORD wCount);
```

**Visual Basic**
```vb
Function McsdStartBuffer(ByVal hDevice As Long, ByVal wCount As Integer) As Long
```

#### 参数说明

| 参数 | 描述 | 范围 |
|---|---|---|
| `hDevice` | 板卡的设备句柄 | - |
| `wCount` | 缓冲函数数量 | 1～16（默认值：1）|

#### 支持的函数

仅以下函数支持批量执行：
- `McsdOutP`
- `McsdDataWrite`

#### 批量执行触发条件

- 缓冲函数数量达到 `wCount` 设定值
- 调用 `McsdStartBuffer` 时存在未执行的缓冲函数
- 调用 `McsdEndBuffer` 时存在未执行的缓冲函数

#### 性能优势

- **最多16个函数批量执行**
- **执行时间约等于1个函数的发送时间**
- 显著提高连续操作的执行效率

### 6-17-3 McsdEndBuffer

**强制执行所有缓冲函数**

#### 函数原型

**C语言**
```c
DWORD McsdEndBuffer(HANDLE hDevice);
```

**Visual Basic**
```vb
Function McsdEndBuffer(ByVal hDevice As Long) As Long
```

#### 功能说明

- 立即执行所有已缓冲的 `McsdOutP` 和 `McsdDataWrite` 函数
- 执行后清空缓冲区，计数器归零
- 适用于需要确保所有操作立即生效的场景

---

## 📚 附录

### 常用参数快速参考

| 参数类型 | 设定范围 | 十六进制 | 备注 |
|---|---|---|---|
| **速度数据** | 1～8,191 | 0x0001～0x1FFF | RANGE/LOW/HIGH SPEED |
| **脉冲数** | 0～16,777,215 | 0x000000～0xFFFFFF | INDEX PULSE DRIVE |
| **S字区间** | 1～4,095 | 0x0001～0x0FFF | SCW-A DATA |
| **缓冲数量** | 1～16 | - | 批量执行函数数 |

### 错误处理建议

1. **始终检查函数返回值**
2. **参考第7章错误代码说明**
3. **使用 `McsdGetStatus` 获取详细状态**
4. **注意驱动状态（BUSY）的影响**

### 最佳实践

1. **初始化顺序**：设备打开 → 参数设定 → 驱动启动
2. **速度调整**：优先使用 `McsdHiSpeedOverride`
3. **批量操作**：使用缓冲功能提高效率
4. **安全停止**：优先使用减速停止，紧急情况使用急停
