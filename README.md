
# PulseFlow : 실시간 제조 장비 모니터링 대시보드

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET_8-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![WPF](https://img.shields.io/badge/WPF-5C2D91?style=for-the-badge&logo=windows&logoColor=white) ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)

산업용 제조 장비의 실시간 상태(온도, 압력)를 모니터링하고 원격으로 제어할 수 있는 풀스택 대시보드 애플리케이션입니다. 

## [실행 영상](https://youtu.be/XDhdYn6EB04?si=ADKfrouun_nevLkr)

> **단기간(3일) Rapid Prototyping**을 통해 전체 시스템 아키텍처를 구상하고, 백엔드 데이터 파이프라인부터 프론트엔드 실시간 렌더링까지 전체 사이클을 구현했습니다.

<br/>

## Tech Stack
* **Frontend:** WPF, WPF UI(Modern Design), LiveCharts (MVVM Pattern)
* **Backend:** ASP.NET Core (Minimal API), Kestrel
* **Database:** PostgreSQL, Entity Framework Core

<br/>

## Key Features
1. **실시간 모니터링 (Real-time Dashboard)**
   - 장비별 온도/압력 센서 데이터 1초 단위 실시간 차트 렌더링
   - UI 스레드 블로킹 없는 데이터 스트리밍
2. **원격 장비 제어 (Remote Control)**
   - API 통신을 통한 개별/전체 기계 가동 및 정지 제어
3. **기간별 통계 분석 (Analytics)**
   - 최근 1시간 / 24시간 / 전체 누적 데이터 필터링
   - Angular Gauge를 활용한 평균 수치 및 직관적인 위험 구간 시각화
4. **MVVM 아키텍처**
   - View와 ViewModel의 분리로 유지보수성 및 확장성 극대화 (`CommunityToolkit.Mvvm` 활용)

<br/>

## Troubleshooting & Optimization (핵심 문제 해결 경험)

프로젝트를 진행하며 겪었던 주요 기술적 병목 현상과 해결 과정입니다.

### 1. 백그라운드 데이터 루프와 웹 서버 간의 스레드 블로킹 해결
* **문제:** 가상 센서 데이터를 생성하는 무한 루프 로직이 메인 스레드를 점유하여, Kestrel 웹 서버가 클라이언트의 시작/정지 제어 명령을 수신하지 못하고 응답을 거부하는 현상 발생.
* **해결:** C#의 버림 연산자(`_ =`)와 `Task.Run`을 결합한 비동기 멀티스레딩 구조 도입. 데이터 생성 무한 루프를 스레드풀의 백그라운드 작업자로 완전히 격리(Fire and Forget)하여, 메인 스레드의 서버 반응성을 확보함.

### 2. 대용량 로그 환경에서의 클라이언트 OOM 방지
* **문제:** 통계 분석 페이지 진입 시, 수백만 건의 데이터를 클라이언트(C#) 메모리로 모두 불러온 뒤(`IEnumerable`) 필터링을 수행할 경우 Out of Memory(OOM)가 발생할 수 있는 잠재적 위험성 식별.
* **해결:** LINQ의 `GroupBy(x => 1)` 테크닉과 내장 집계 함수를 조합하여, 연산 자체를 PostgreSQL 데이터베이스 엔진 내에서 처리하도록 리팩토링. 클라이언트는 1줄의 집계 결과만 전송받게 하여 메모리 사용량과 네트워크 대역폭 낭비를 단축.

### 3. 실시간 차트의 UX 개선 및 NaN 처리
* **문제:** 기계 정지 시 데이터 유입이 중단되면 차트의 시간 흐름이 정지하여 시스템 다운으로 오인될 수 있는 UX 결함 발견.
* **해결:** 장비 정지 상태를 감지할 경우, 차트 렌더링 배열에 의도적으로 `double.NaN`을 연속 삽입하는 로직 구축. LiveCharts가 NaN을 투명 공간으로 처리하는 특성을 활용하여, "데이터 없음(정지 상태)"을 명확히 시각화하면서도 타임라인 자체는 정상적으로 흐르도록 모니터링 신뢰성 확보.

<br/>

## Getting Started (실행 방법)
### 1. 데이터베이스 구축 (PostgreSQL 세팅)
본 프로젝트는 초 단위로 누적되는 대용량 시계열 센서 데이터의 조회 성능을 극대화하고 아카이빙(Archiving)을 용이하게 하기 위해, PostgreSQL 월별 테이블 파티셔닝(Table Partitioning by Month)을 기본 아키텍처로 채택하고 있습니다.

DB 환경(pgAdmin 등)에 접속하여 먼저 마스터 테이블과 해당 월의 파티션 테이블을 생성합니다.

```sql
-- 1) 마스터 테이블 생성 (LoggedAt 기준으로 파티션 지정)
CREATE TABLE "SensorLogs" (
    "Id" SERIAL,
    "MachineName" TEXT NOT NULL,
    "Temperature" DOUBLE PRECISION NOT NULL,
    "Pressure" DOUBLE PRECISION NOT NULL,
    "LoggedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    PRIMARY KEY ("Id", "LoggedAt")
) PARTITION BY RANGE ("LoggedAt");

-- 2) 예시: 특정 월의 파티션 테이블 생성 (예: 2026년 6월)
CREATE TABLE "SensorLogs_202606" 
    PARTITION OF "SensorLogs" 
    FOR VALUES FROM ('2026-06-01 00:00:00+00') TO ('2026-07-01 00:00:00+00');

```

<img width="1736" height="1046" alt="1" src="https://github.com/user-attachments/assets/f2f34ea5-67da-4d68-b7bf-65397796ca0d" />


이러한 구조로 되어 있습니다.


### 2. 패키지 설치 및 DbContext 설정
#### 1) 필수 Nuget 패키지 설치
Install-Package Npgsql
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL

#### 2) 로컬 DB 인프라 반영
Scaffold-DbContext "Host=localhost;Database=PulseFlowDB;Username=myusername;Password=mypassword" Npgsql.EntityFrameworkCore.PostgreSQL -o Models
Database와 Username, Password를 설정 했던 것들로 맞춰주세요.

#### 3) 코드 수정
PulseFlow.Simulator의 Program.cs에서
```
    string connString = "Host=localhost;Database=PulseFlowDB;Username=myUsername;Password=myPassword";

```
를 수정해주세요.
