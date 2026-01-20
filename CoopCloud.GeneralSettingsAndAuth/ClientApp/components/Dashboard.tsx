import React from "react";
import {
  Shield,
  FileText,
  ArrowDown,
  ChevronDown,
  CheckCircle,
} from "lucide-react";
import {
  Card,
  Badge,
  Button,
  CardTitle,
  CardAction,
  CardHeader,
  CardContent,
} from "@nubeteck/shadcn";
import {
  Pie,
  Bar,
  Area,
  Cell,
  XAxis,
  YAxis,
  AreaChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip as RechartsTooltip,
  PieChart as RechartsPieChart,
  BarChart as RechartsBarChart,
  // LineChart as RechartsLineChart,
} from "recharts";

export function Dashboard() {
  // Color palette using system colors: green-600 and yellow-400
  const colors = {
    info: "#9ca3af", // gray-400 - soft pleasant gray (changed from blue)
    pink: "#facc15", // yellow-400 (changed from red)
    teal: "#0d9488", // teal-600
    green: "#16a34a", // green-600
    danger: "#facc15", // yellow-400 (changed from red)
    purple: "#9333ea", // purple-600
    orange: "#ea580c", // orange-600
    primary: "#16a34a", // green-600 - Primary system color
    success: "#16a34a", // green-600
    warning: "#facc15", // yellow-400
    tertiary: "#16a34a", // green-600 for consistency
    secondary: "#facc15", // yellow-400 - Secondary system color
    quaternary: "#facc15", // yellow-400 for balance
  };

  // Data for performance chart (main area chart)
  const performanceData = [
    { day: "Mon", socios: 45, prestamos: 42, aportaciones: 35 },
    { day: "Tue", socios: 52, prestamos: 38, aportaciones: 48 },
    { day: "Wed", socios: 48, prestamos: 45, aportaciones: 42 },
    { day: "Thu", socios: 58, prestamos: 48, aportaciones: 52 },
    { day: "Fri", socios: 55, prestamos: 52, aportaciones: 48 },
    { day: "Sat", socios: 62, prestamos: 48, aportaciones: 55 },
    { day: "Sun", socios: 58, prestamos: 55, aportaciones: 52 },
    { day: "Mon", socios: 65, prestamos: 62, aportaciones: 58 },
    { day: "Tue", socios: 62, prestamos: 58, aportaciones: 55 },
  ];

  // Data for company growth (bar chart)
  const growthData = [
    { month: "E", budget: 900, actual: 1560, target: 1200 },
    { month: "F", actual: 780, budget: 850, target: 1100 },
    { month: "M", budget: 800, actual: 1560, target: 1000 },
  ];

  // Data for income circular chart - updated with system colors
  const incomeData = [
    {
      value: 46.3,
      amount: 7126.49,
      name: "Aportaciones",
      color: colors.primary,
    }, // green-600
    {
      value: 32.6,
      amount: 5017.79,
      name: "Préstamos",
      color: colors.secondary,
    }, // yellow-400
    { value: 22.1, amount: 3401.62, name: "Servicios", color: colors.info }, // blue for contrast
  ];

  const totalIncome = incomeData.reduce((sum, item) => sum + item.amount, 0);

  // Pie chart data for distribution - updated with system colors
  const distributionData = [
    { value: 40, color: colors.primary, name: "Socios activos" }, // green-600
    { value: 30, name: "Socios nuevos", color: colors.secondary }, // yellow-400
    { value: 30, color: colors.info, name: "Socios inactivos" }, // blue for contrast
  ];

  // Progress data for company progress
  const progressData = [
    {
      progress: 85,
      percentage: 85,
      name: "Registro de socios",
      status: "Excelente progreso",
    },
    {
      progress: 72,
      percentage: 72,
      status: "En progreso activo",
      name: "Verificación de documentos",
    },
    {
      progress: 94,
      percentage: 94,
      name: "Perfiles de riesgo",
      status: "Meta casi completada",
    },
    {
      progress: 58,
      percentage: 58,
      status: "Requiere atención",
      name: "Reportes regulatorios",
    },
  ];

  // Company performance data
  // const companyData = [
  //   {
  //     name: "Agencia Dos",
  //     amount: "$ 1.234.566",
  //     sparkline: [20, 35, 25, 40, 30, 45, 35, 50],
  //   },
  //   {
  //     name: "Surya Dos",
  //     amount: "$ 1.234.566",
  //     sparkline: [30, 25, 40, 35, 50, 45, 55, 60],
  //   },
  //   {
  //     name: "Lermonio Dos",
  //     amount: "$ 1.234.566",
  //     sparkline: [50, 45, 40, 35, 30, 25, 20, 15],
  //   },
  //   {
  //     name: "Filayudal Dos",
  //     amount: "$ 1.234.566",
  //     sparkline: [15, 25, 35, 30, 40, 45, 50, 55],
  //   },
  // ];

  // const MiniSparkline = ({
  //   data,
  //   color = colors.primary,
  // }: {
  //   data: number[];
  //   color?: string;
  // }) => (
  //   <div className="w-16 h-8">
  //     <ResponsiveContainer width="100%" height="100%">
  //       <RechartsLineChart
  //         data={data.map((value, index) => ({ value, index }))}
  //       >
  //         <Line
  //           dot={false}
  //           stroke={color}
  //           type="monotone"
  //           dataKey="value"
  //           strokeWidth={2}
  //         />
  //       </RechartsLineChart>
  //     </ResponsiveContainer>
  //   </div>
  // );

  return (
    <div className="min-h-full">
      <div className="space-y-6">
        {/* Analytics Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900 ">
              Análisis de rendimiento
            </h2>
            <div className="flex items-center gap-2">
              <Badge
                variant="outline"
                className="text-xs px-3 py-1 bg-yellow-50 text-yellow-700 border-yellow-200"
              >
                Tiempo real
              </Badge>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Data Performance Company */}
            <div className="lg:col-span-2">
              <Card className="border-0 shadow-sm bg-gradient-to-br from-white to-green-50/30">
                <CardHeader className="pb-4">
                  <div className="flex flex-col space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <CardTitle className="text-lg font-semibold text-gray-900 ">
                          Rendimiento de socios
                        </CardTitle>
                        <p className="text-sm text-gray-600 mt-1">
                          Evolución de actividad y crecimiento
                        </p>
                      </div>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs hover:bg-green-50"
                      >
                        <ChevronDown className="mr-1 h-3 w-3" />
                        Ordenar
                      </Button>
                    </div>
                    <div className="flex items-center gap-2 flex-wrap">
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs text-gray-600 hover:bg-green-50"
                      >
                        12 meses
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs text-gray-600 hover:bg-green-50"
                      >
                        30 días
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs text-gray-600 hover:bg-green-50"
                      >
                        7 días
                      </Button>
                      <Button
                        size="sm"
                        className="text-xs bg-green-600 hover:bg-green-700 text-white"
                      >
                        24 horas
                      </Button>
                    </div>
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="flex items-center gap-6 mb-4">
                    <div className="text-2xl font-semibold">$4k</div>
                    <div className="text-2xl font-semibold">$3k</div>
                    <div className="text-2xl font-semibold">$2k</div>
                    <div className="text-2xl font-semibold">$1k</div>
                    <div className="text-2xl font-semibold">$0</div>
                  </div>
                  <div className="h-64 relative">
                    <ResponsiveContainer width="100%" height="100%">
                      <AreaChart data={performanceData}>
                        <defs>
                          <linearGradient
                            x1="0"
                            y1="0"
                            x2="0"
                            y2="1"
                            id="colorSocios"
                          >
                            <stop
                              offset="5%"
                              stopOpacity={0.3}
                              stopColor={colors.primary}
                            />
                            <stop
                              offset="95%"
                              stopOpacity={0}
                              stopColor={colors.primary}
                            />
                          </linearGradient>
                          <linearGradient
                            x1="0"
                            y1="0"
                            x2="0"
                            y2="1"
                            id="colorAportaciones"
                          >
                            <stop
                              offset="5%"
                              stopOpacity={0.3}
                              stopColor={colors.pink}
                            />
                            <stop
                              offset="95%"
                              stopOpacity={0}
                              stopColor={colors.pink}
                            />
                          </linearGradient>
                        </defs>
                        <CartesianGrid
                          opacity={0.5}
                          stroke="#e5e7eb"
                          strokeDasharray="3 3"
                        />
                        <XAxis
                          dataKey="day"
                          axisLine={false}
                          tickLine={false}
                          tick={{ fontSize: 12, fill: "#6b7280" }}
                        />
                        <YAxis hide />
                        <RechartsTooltip
                          content={({ label, active, payload }) => {
                            if (active && payload && payload.length) {
                              return (
                                <div className="bg-white p-3 rounded-lg shadow-lg border text-sm">
                                  <p className="font-medium text-gray-900">
                                    {label}
                                  </p>
                                  {payload.map((entry, index) => (
                                    <p
                                      key={index}
                                      style={{ color: entry.color }}
                                    >
                                      {entry.name}: {entry.value}
                                    </p>
                                  ))}
                                </div>
                              );
                            }
                            return null;
                          }}
                        />
                        <Area
                          type="monotone"
                          strokeWidth={3}
                          fillOpacity={1}
                          dataKey="socios"
                          name="Socios activos"
                          stroke={colors.primary}
                          fill="url(#colorSocios)"
                        />
                        <Area
                          type="monotone"
                          strokeWidth={3}
                          fillOpacity={1}
                          name="Aportaciones"
                          stroke={colors.pink}
                          dataKey="aportaciones"
                          fill="url(#colorAportaciones)"
                        />
                      </AreaChart>
                    </ResponsiveContainer>

                    {/* Floating tooltips */}
                    <div className="absolute top-4 left-12 bg-green-600 text-white px-3 py-2 rounded-lg text-sm font-medium shadow-lg">
                      Socios activos
                    </div>
                    <div className="absolute top-16 right-20 bg-yellow-500 text-gray-900 px-3 py-2 rounded-lg text-sm font-medium shadow-lg">
                      Aportaciones
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Company Growth Selling */}
            <Card className="border-0 shadow-sm bg-gradient-to-br from-white to-yellow-50/30">
              <CardHeader className="pb-4">
                <div className="flex flex-col space-y-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <CardTitle className="text-lg font-semibold text-gray-900 ">
                        Crecimiento mensual
                      </CardTitle>
                      <p className="text-sm text-gray-600 mt-1">
                        Comparativo trimestral
                      </p>
                    </div>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="text-xs hover:bg-yellow-50"
                    >
                      <ChevronDown className="mr-1 h-3 w-3" />
                      Filtrar
                    </Button>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                {/* Top values aligned with main chart */}
                <div className="flex justify-between items-end mb-4">
                  <div className="text-center">
                    <div className="text-2xl font-semibold text-gray-900">
                      1,560
                    </div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-semibold text-gray-900">
                      780
                    </div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-semibold text-gray-900">
                      1,560
                    </div>
                  </div>
                </div>

                {/* Chart container with increased height to match main chart */}
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <RechartsBarChart data={growthData}>
                      <Bar
                        dataKey="actual"
                        fill={colors.primary}
                        radius={[4, 4, 0, 0]}
                      />
                      <Bar
                        dataKey="target"
                        fill={colors.pink}
                        radius={[4, 4, 0, 0]}
                      />
                      <Bar
                        dataKey="budget"
                        fill={colors.info}
                        radius={[4, 4, 0, 0]}
                      />
                    </RechartsBarChart>
                  </ResponsiveContainer>
                </div>

                {/* Month labels positioned to align with main chart days */}
                <div className="flex justify-between mt-4 text-xs text-gray-600">
                  <span className="">Ene</span>
                  <span className="">Feb</span>
                  <span className="">Mar</span>
                </div>
              </CardContent>
            </Card>
          </div>
        </section>

        {/* Widgets Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900 ">
              Métricas operacionales
            </h2>
            <div className="flex items-center gap-2">
              <Badge
                variant="outline"
                className="text-xs px-3 py-1 bg-green-50 text-green-700 border-green-200"
              >
                2 indicadores
              </Badge>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            {/* Rendimiento de Datos - Professional Style */}
            <Card className="border shadow-md bg-white hover:shadow-lg transition-all duration-300">
              <CardHeader className="flex items-center justify-between">
                <CardTitle>Rendimiento de datos</CardTitle>
                <Button size="sm" variant="ghost">
                  <ChevronDown className="mr-1 h-3 w-3" />
                  Filtros
                </Button>
              </CardHeader>

              <CardContent className="px-6 pb-6">
                <div className="flex items-center justify-center mb-3">
                  <div className="relative w-52 h-52">
                    <ResponsiveContainer width="100%" height="100%">
                      <RechartsPieChart>
                        <Pie
                          cx="50%"
                          cy="50%"
                          dataKey="value"
                          strokeWidth={1}
                          outerRadius={90}
                          paddingAngle={1}
                          stroke="#ffffff"
                          data={distributionData}
                        >
                          {distributionData.map((entry, index) => (
                            <Cell fill={entry.color} key={`cell-${index}`} />
                          ))}
                        </Pie>
                        <RechartsTooltip
                          content={({ active, payload }) => {
                            if (active && payload && payload.length) {
                              return (
                                <div className="bg-white/95 backdrop-blur-sm p-3 rounded-lg shadow-lg border border-gray-100 text-sm">
                                  <p className="font-medium text-gray-900">
                                    {payload[0].name}
                                  </p>
                                  <p
                                    className="text-xs"
                                    style={{ color: payload[0].color }}
                                  >
                                    {payload[0].value}%
                                  </p>
                                </div>
                              );
                            }
                            return null;
                          }}
                        />
                      </RechartsPieChart>
                    </ResponsiveContainer>
                  </div>
                </div>

                <div className="space-y-2">
                  {distributionData.map((item, index) => (
                    <div
                      key={index}
                      className="flex items-center justify-between p-2 bg-gray-50/70 rounded-lg border border-gray-100/50 hover:bg-white hover:shadow-sm transition-all duration-200"
                    >
                      <div className="flex items-center gap-2">
                        <div
                          className="w-2.5 h-2.5 rounded-full shadow-sm"
                          style={{
                            backgroundColor: item.color,
                            boxShadow: `0 1px 4px ${item.color}20`,
                          }}
                        />
                        <span className="text-xs font-medium text-gray-800 ">
                          {item.name}
                        </span>
                      </div>
                      <div className="text-right">
                        <div className="text-sm font-bold text-gray-900">
                          {item.value}%
                        </div>
                        <div className="text-xs text-gray-600">
                          Unidades: {item.value * 10}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            {/* Ingresos - Professional Style with Donut Chart */}
            <Card className="border shadow-md bg-white hover:shadow-lg transition-all duration-300">
              <CardHeader className="flex items-center justify-between">
                <CardTitle>Ingresos totales</CardTitle>
                <div className="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded-md border">
                  Todo el tiempo
                </div>
              </CardHeader>

              <CardContent className="px-6 pb-6">
                <div className="flex items-center justify-center mb-3">
                  <div className="relative w-52 h-52">
                    <ResponsiveContainer width="100%" height="100%">
                      <RechartsPieChart>
                        <Pie
                          cx="50%"
                          cy="50%"
                          dataKey="value"
                          strokeWidth={1}
                          innerRadius={45}
                          outerRadius={90}
                          paddingAngle={2}
                          stroke="#ffffff"
                          data={incomeData}
                        >
                          {incomeData.map((entry, index) => (
                            <Cell fill={entry.color} key={`cell-${index}`} />
                          ))}
                        </Pie>
                        <RechartsTooltip
                          content={({ active, payload }) => {
                            if (active && payload && payload.length) {
                              return (
                                <div className="bg-white/95 backdrop-blur-sm p-3 rounded-lg shadow-lg border border-gray-100 text-sm">
                                  <p className="font-medium text-gray-900">
                                    {payload[0].name}
                                  </p>
                                  <p
                                    className="text-xs"
                                    style={{ color: payload[0].color }}
                                  >
                                    $
                                    {payload[0].payload.amount.toLocaleString()}
                                  </p>
                                  <p className="text-gray-600 text-xs">
                                    {payload[0].value}%
                                  </p>
                                </div>
                              );
                            }
                            return null;
                          }}
                        />
                      </RechartsPieChart>
                    </ResponsiveContainer>

                    {/* Center text for total income */}
                    <div className="absolute inset-0 flex items-center justify-center">
                      <div className="text-center">
                        <div className="text-xl font-bold text-gray-900">
                          ${totalIncome.toLocaleString()}
                        </div>
                        <div className="text-xs text-gray-500 ">Total</div>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  {incomeData.map((item, index) => (
                    <div
                      key={index}
                      className="flex items-center justify-between p-2 bg-gray-50/70 rounded-lg border border-gray-100/50 hover:bg-white hover:shadow-sm transition-all duration-200"
                    >
                      <div className="flex items-center gap-2">
                        <div
                          className="w-2.5 h-2.5 rounded-full shadow-sm"
                          style={{
                            backgroundColor: item.color,
                            boxShadow: `0 1px 4px ${item.color}20`,
                          }}
                        />
                        <span className="text-xs font-medium text-gray-800 ">
                          {item.name}
                        </span>
                      </div>
                      <div className="text-right">
                        <div className="text-sm font-bold text-gray-900">
                          ${item.amount.toLocaleString()}
                        </div>
                        <div className="text-xs text-gray-600">
                          {item.value}%
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>
        </section>

        {/* Progreso de Objetivos Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900 ">
              Progreso de objetivos
            </h2>
            <Badge
              variant="outline"
              className="text-xs px-3 py-1 bg-green-50 text-green-700 border-green-200"
            >
              En progreso
            </Badge>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-4 gap-6">
            {progressData.map((item, index) => {
              const progressColor = "#16a34a"; // Verde para todos los elementos

              return (
                <Card
                  key={index}
                  style={{ borderRadius: "0.625rem" }}
                  className="border-0 shadow-sm bg-white hover:shadow-lg transition-shadow duration-300"
                >
                  <CardContent className="">
                    <div className="flex items-center justify-between mb-4">
                      <div className="flex-1 min-w-0">
                        <h3 className="font-semibold text-gray-900 text-sm ">
                          {item.name}
                        </h3>
                      </div>
                      <div className="text-right ml-3">
                        <div
                          className="text-xl font-bold"
                          style={{ color: progressColor }}
                        >
                          {item.percentage}%
                        </div>
                        <div className="text-xs text-gray-500 ">Completado</div>
                      </div>
                    </div>

                    {/* Simple progress bar */}
                    <div className="w-full bg-gray-200 rounded-full h-2.5">
                      <div
                        className="h-full rounded-full transition-all duration-1000 ease-out"
                        style={{
                          width: `${item.progress}%`,
                          backgroundColor: progressColor,
                        }}
                      />
                    </div>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </section>

        {/* Transacciones Recientes y Alertas Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900 ">
              Actividad del sistema
            </h2>
            <div className="flex items-center gap-2">
              <Badge
                variant="outline"
                className="text-xs px-3 py-1 bg-green-50 text-green-700 border-green-200"
              >
                Tiempo real
              </Badge>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Transacciones Recientes */}
            <Card className="border-0 shadow-sm bg-white">
              <CardHeader className="flex items-center justify-between">
                <CardTitle>Transacciones recientes</CardTitle>
                <CardAction>
                  <Button
                    size="sm"
                    variant="ghost"
                    className="text-xs text-gray-600 hover:bg-gray-50 px-2 py-1"
                  >
                    Ver todas
                  </Button>
                </CardAction>
              </CardHeader>

              <CardContent className="p-0">
                <table className="w-full">
                  <thead>
                    <tr className="border-b bg-gray-50/50">
                      <th className="text-left px-4 py-3 font-medium text-gray-700 text-xs">
                        Usuario
                      </th>
                      <th className="text-left px-3 py-3 font-medium text-gray-700 text-xs">
                        Monto
                      </th>
                      <th className="text-left px-3 py-3 font-medium text-gray-700 text-xs">
                        Estado
                      </th>
                      <th className="text-right px-4 py-3 font-medium text-gray-700 text-xs">
                        Tiempo
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {[
                      {
                        avatar: "MG",
                        id: "TXN-001",
                        type: "Depósito",
                        time: "Hace 2 min",
                        name: "María García",
                        amount: "RD$ 15,000",
                        status: "Completado",
                        statusColor: "bg-green-100 text-green-700",
                      },
                      {
                        avatar: "JP",
                        id: "TXN-002",
                        type: "Retiro",
                        name: "Juan Pérez",
                        time: "Hace 5 min",
                        amount: "RD$ 8,500",
                        status: "Pendiente",
                        statusColor: "bg-yellow-100 text-yellow-700",
                      },
                      {
                        avatar: "AR",
                        id: "TXN-003",
                        time: "Hace 12 min",
                        amount: "RD$ 25,000",
                        status: "Completado",
                        name: "Ana Rodríguez",
                        type: "Transferencia",
                        statusColor: "bg-green-100 text-green-700",
                      },
                      {
                        avatar: "CM",
                        id: "TXN-004",
                        type: "Depósito",
                        status: "Revisión",
                        time: "Hace 18 min",
                        amount: "RD$ 50,000",
                        name: "Carlos Martínez",
                        statusColor: "bg-gray-100 text-gray-700",
                      },
                      {
                        avatar: "LS",
                        id: "TXN-005",
                        type: "Retiro",
                        time: "Hace 25 min",
                        name: "Laura Santos",
                        amount: "RD$ 12,300",
                        status: "Completado",
                        statusColor: "bg-green-100 text-green-700",
                      },
                      {
                        avatar: "RJ",
                        id: "TXN-006",
                        type: "Depósito",
                        time: "Hace 32 min",
                        amount: "RD$ 35,750",
                        status: "Procesando",
                        name: "Roberto Jiménez",
                        statusColor: "bg-orange-100 text-orange-700",
                      },
                    ].map((transaction, index) => (
                      <tr
                        key={index}
                        className="border-b last:border-b-0 hover:bg-gray-50/30 transition-colors duration-200"
                      >
                        <td className="px-4 py-3">
                          <div className="flex items-center gap-3">
                            <div className="w-7 h-7 rounded-full bg-green-100 flex items-center justify-center">
                              <span className="text-xs font-medium text-green-700 ">
                                {transaction.avatar}
                              </span>
                            </div>
                            <div>
                              <div className="text-xs font-medium text-gray-900 ">
                                {transaction.name}
                              </div>
                              <div className="text-xs text-gray-500">
                                {transaction.id}
                              </div>
                            </div>
                          </div>
                        </td>
                        <td className="px-3 py-3">
                          <div className="text-xs font-medium text-gray-900">
                            {transaction.amount}
                          </div>
                        </td>
                        <td className="px-3 py-3">
                          <span
                            className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${transaction.statusColor}`}
                          >
                            {transaction.status}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-right">
                          <div className="text-xs text-gray-500 ">
                            {transaction.time}
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {/* Paginación */}
                <div className="flex items-center justify-between px-4 py-3 border-t bg-gray-50/30">
                  <div className="flex items-center gap-2">
                    <span className="text-xs text-gray-600 ">
                      Mostrando 6 de 147
                    </span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      disabled
                      size="sm"
                      variant="ghost"
                      className="text-xs px-3 py-1 h-7 disabled:opacity-50"
                    >
                      <ArrowDown className="h-3 w-3 mr-1 rotate-90" />
                      Anterior
                    </Button>
                    <div className="flex items-center gap-1">
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs px-2 py-1 h-7 bg-green-100 text-green-700"
                      >
                        1
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs px-2 py-1 h-7"
                      >
                        2
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs px-2 py-1 h-7"
                      >
                        3
                      </Button>
                      <span className="text-xs text-gray-400 px-1">...</span>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-xs px-2 py-1 h-7"
                      >
                        30
                      </Button>
                    </div>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="text-xs px-3 py-1 h-7"
                    >
                      Siguiente
                      <ArrowDown className="h-3 w-3 ml-1 -rotate-90" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Alertas del Sistema */}
            <Card className="shadow-none">
              <CardHeader>
                <CardTitle>Alertas del sistema</CardTitle>
                {/* <CardDescription>Notificaciones importantes</CardDescription> */}
                <CardAction>
                  <Button size="sm" variant="ghost">
                    Ver todas
                  </Button>
                </CardAction>
              </CardHeader>

              <CardContent className="px-4 pb-4">
                {/* Today Header */}
                <div className="flex items-center justify-between mb-4">
                  <div>
                    <div className="text-xs text-gray-500 ">Hoy</div>
                    <div className="text-lg font-bold text-gray-900 ">
                      21 ago
                    </div>
                  </div>
                  <Button
                    size="sm"
                    variant="ghost"
                    className="text-xs text-gray-600 hover:bg-gray-50"
                  >
                    Mostrar todo
                  </Button>
                </div>

                {/* Alerts List */}
                <div className="space-y-3">
                  {[
                    {
                      time: "19:00",
                      priority: "high",
                      title: "Documentos próximos a vencer",
                      icon: <CheckCircle className="h-4 w-4" />,
                      description:
                        "Revisar 5 socios con documentos que vencen esta semana",
                    },
                    {
                      time: "21:15",
                      priority: "medium",
                      icon: <Shield className="h-4 w-4" />,
                      title: "Verificación de riesgo pendiente",
                      description: "Evaluar perfiles de riesgo actualizados",
                    },
                    {
                      time: "22:42",
                      priority: "low",
                      title: "Reporte mensual disponible",
                      icon: <FileText className="h-4 w-4" />,
                      description:
                        "Generar reporte de actividades para IDECOOP",
                    },
                  ].map((alert, index) => (
                    <div
                      key={index}
                      className="flex items-start gap-3 p-3 rounded-lg bg-gray-50/50 hover:bg-gray-100/50 transition-colors duration-200"
                    >
                      <div className="flex items-center justify-center w-8 h-8 rounded-lg bg-green-100 text-green-600 shrink-0">
                        {alert.icon}
                      </div>

                      <div className="flex-1 min-w-0">
                        <div className="flex items-start justify-between gap-2">
                          <div className="flex-1">
                            <div className="text-xs font-medium text-gray-900 ">
                              {alert.title}
                            </div>
                            <div className="text-xs text-gray-600 mt-1">
                              {alert.description}
                            </div>
                          </div>
                          <div className="text-xs text-gray-500 whitespace-nowrap">
                            {alert.time}
                          </div>
                        </div>

                        <div className="flex items-center justify-between mt-2">
                          <div
                            className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                              alert.priority === "high"
                                ? "bg-yellow-100 text-yellow-700"
                                : alert.priority === "medium"
                                  ? "bg-yellow-100 text-yellow-700"
                                  : "bg-gray-100 text-gray-700"
                            }`}
                          >
                            {alert.priority === "high"
                              ? "Alta"
                              : alert.priority === "medium"
                                ? "Media"
                                : "Baja"}
                          </div>

                          <Button
                            size="sm"
                            variant="ghost"
                            className="text-xs text-gray-500 hover:text-gray-700 px-2 py-1 h-6"
                          >
                            Marcar como leída
                          </Button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Footer */}
                <div className="mt-4 pt-3 border-t">
                  <div className="flex items-center justify-between text-xs text-gray-500">
                    <span>3 alertas pendientes</span>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="text-xs text-gray-600 hover:bg-gray-50 px-2 py-1 h-6"
                    >
                      Configurar alertas
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </section>
      </div>
    </div>
  );
}
