# Microsupport Quick Pro Specification

**Resolutions:** movement resolution for each axis (μm/pulse)

* `axisX`: X-axis resolution
* `axisY`: Y-axis resolution
* `axisZ`: Z-axis resolution

# API Specification (v1.1)

## 1. Controller Server (WinForms C# APP)

### 1.1 Overview

The Controller Server listens for TCP requests from the PathPlanner Server and controls two manipulators. All messages are plain-text, comma-separated. On error, the server responds with:

```text
ERROR, <code>, <message>
```

### 1.2 Methods

#### SendStatus(id1, id2)

* **Trigger:** `GET_STATUS, <id1>, <id2>`
* **Action:**

  1. Validate `id1` and `id2`.
  2. Read current displacement `(X, Y, Z)` from center for each manipulator.
* **Response:**

  ```text
  STATUS, <id1>, <X1>, <Y1>, <Z1>, <id2>, <X2>, <Y2>, <Z2>
  ```

#### StepInc(id1, id2, X, Y, Z)

* **Trigger:** `START_STEP, <id1>, <id2>, <X>, <Y>, <Z>`
* **Action:**

  1. Validate manipulator IDs and bounds of `(X, Y, Z)`.
  2. Perform incremental move: steps = value / resolution per axis.
  3. On success, call `SendStatus(id1, id2)`.
* **Response:**

  ```text
  STEP_COMPLETED, <id1>, <id2>
  ```

#### ProcessPathData(rawData)

* **Trigger:** `PATH_DATA, <payload>`
* **Action:**

  1. Parse payload into sequence of 6-tuples: `[X1, Y1, Z1, X2, Y2, Z2]...`.
  2. Validate format and count.
  3. Store internally.
* **Response:**

  ```text
  PATH_DATA_RECEIVED
  ```

#### PathTracking(id1, id2)

* **Trigger:** `START_PATH, <id1>, <id2>`
* **Action:**

  1. For each time step in stored path:

     * Extract `(X1, Y1, Z1, X2, Y2, Z2)`.
     * Call `StepInc(id1, id2, X1, Y1, Z1)` and `StepInc(id1, id2, X2, Y2, Z2)` or batching as needed.
  2. On any failure, abort and send error.
  3. On completion, call `SendStatus(id1, id2)`.
* **Response:**

  ```text
  PATH_COMPLETED, <id1>, <id2>
  ```

### 1.3 Request → Response Map

| Request                               | Response                                             |
| ------------------------------------- | ---------------------------------------------------- |
| `HEARTBEAT`                           | `HEARTBEAT_OK`                                       |
| `GET_STATUS, <id1>, <id2>`            | `STATUS, <id1>, <X1>,<Y1>,<Z1>,<id2>,<X2>,<Y2>,<Z2>` |
| `START_STEP, <id1>,<id2>,<X>,<Y>,<Z>` | `STEP_COMPLETED, <id1>, <id2>`                       |
| `PATH_DATA, <payload>`                | `PATH_DATA_RECEIVED`                                 |
| `START_PATH, <id1>, <id2>`            | `PATH_COMPLETED, <id1>, <id2>`                       |
| *invalid or unknown request*          | `ERROR, <code>, <message>`                           |

---

## 2. PathPlanner Server (MATLAB APP)

### 2.1 Overview

The PathPlanner Server computes kinematics and coordinates with Controller. All messages are plain-text, comma-separated. On error, it sends:

```text
ERROR, <code>, <message>
```

### 2.2 Properties

* `X0, Y0, Z0`            — current end-effector position
* `Phi0, Theta0, Psi0`    — current end-effector orientation

### 2.3 Methods

#### GetStatus(id1, id2)

1. Send `GET_STATUS, <id1>, <id2>`.
2. Parse response `STATUS, <id1>,<X1>,<Y1>,<Z1>,<id2>,<X2>,<Y2>,<Z2>`.
3. On parse error or timeout, handle error.
4. Return `[(X1,Y1,Z1),(X2,Y2,Z2)]`.

#### StepMove(id1, id2, X, Y, Z)

* Send `START_STEP, <id1>,<id2>,<X>,<Y>,<Z>`.
* Await `STEP_COMPLETED, <id1>,<id2>` or error.

#### onCalcFK()

1. Call `GetStatus(id1,id2)`.
2. Call `SolveFK(X1, Y1, Z1, X2, Y2, Z2, model, params)`.
3. Update `X0, Y0, Z0, Phi0, Theta0, Psi0`.

#### onCalcIK(Xt, Yt, Zt, Phit, Thetat, Psit)

1. Use current pose `(X0...Psi0)`.
2. Call `SolveIK(X0, Y0, Z0, Phi0,...,Psit)`.
3. Return trajectory array `N×6`.

#### PlanPath(id1, id2)

1. Invoke `onCalcFK()`.
2. Invoke `onCalcIK(...)` to get trajectory.
3. Pack into payload:

   ```text
   PATH_DATA,
   X11,Y11,Z11,X12,Y12,Z12, ...
   ```
4. Send `PATH_DATA, <payload>` and await `PATH_DATA_RECEIVED`.

#### ExecutePath(id1, id2)

* Triggered when `PATH_DATA_RECEIVED` arrives.
* Send `START_PATH, <id1>, <id2>`.
* Await `PATH_COMPLETED`.

### 2.4 Controller → MATLAB Map

| From Controller      | MATLAB Action / Response                  |
| -------------------- | ----------------------------------------- |
| `HEARTBEAT`          | reply `HEARTBEAT_OK`                      |
| `STATUS,...`         | update internal state; no immediate reply |
| `STEP_COMPLETED,...` | optionally send next `GET_STATUS`         |
| `PATH_DATA_RECEIVED` | enable `ExecutePath()`                    |
| `PATH_COMPLETED,...` | disable `ExecutePath()`                   |
| *invalid or timeout* | send `ERROR, <code>, <message>`           |

---

## 3. Error Handling

### 3.1 Format

```text
ERROR, <code>, <message>
```

### 3.2 Error Codes

* `100` – Unknown request
* `101` – Invalid parameters
* `102` – Manipulator ID out of range
* `103` – Trajectory parse failure
* `104` – Motion execution failure
* `105` – Timeout waiting for response

### 3.3 Behavior

* Validate all inputs; on failure, respond with `ERROR, code, message` immediately.
* Log errors with timestamp and full payload.
* Gracefully abort long-running operations on error and notify peer.
* Support future protocol versioning via optional `v<major>.<minor>` prefix.
