# MonitorRecall のアプリアイコン (src/MonitorRecall/app.ico) を生成する
# 図柄: 2 台のモニター + 手前の画面に「元に戻す」円形矢印
Add-Type -AssemblyName System.Drawing

$out = Join-Path $PSScriptRoot '..\src\MonitorRecall\app.ico'
$sizes = 16, 20, 24, 32, 40, 48, 64, 128, 256

function RoundRect([float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
    $p = New-Object Drawing.Drawing2D.GraphicsPath
    $d = $r * 2
    $p.AddArc($x, $y, $d, $d, 180, 90)
    $p.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $p.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $p.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $p.CloseFigure()
    $p
}

function Draw-Monitor($g, [float]$x, [float]$y, [float]$w, [float]$h, $bezel, $screenTop, $screenBottom) {
    $b = New-Object Drawing.SolidBrush $bezel
    # スタンド
    $g.FillRectangle($b, $x + $w / 2 - $w * 0.07, $y + $h - 2, $w * 0.14, $h * 0.2)
    $g.FillPath($b, (RoundRect ($x + $w / 2 - $w * 0.24) ($y + $h + $h * 0.16) ($w * 0.48) ($h * 0.1) ($h * 0.05)))
    # 本体と画面
    $g.FillPath($b, (RoundRect $x $y $w $h ($w * 0.07)))
    $inset = $w * 0.065
    $rect = New-Object Drawing.RectangleF ($x + $inset), ($y + $inset), ($w - $inset * 2), ($h - $inset * 2)
    $sb = New-Object Drawing.Drawing2D.LinearGradientBrush $rect, $screenTop, $screenBottom, 90
    $g.FillPath($sb, (RoundRect $rect.X $rect.Y $rect.Width $rect.Height ($w * 0.03)))
}

function Render([int]$size) {
    $bmp = New-Object Drawing.Bitmap $size, $size, ([Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $g.PixelOffsetMode = 'HighQuality'
    $g.Clear([Drawing.Color]::Transparent)
    $g.ScaleTransform($size / 256.0, $size / 256.0)

    # 奥のモニター
    Draw-Monitor $g 10 34 140 100 ([Drawing.Color]::FromArgb(84, 110, 122)) `
        ([Drawing.Color]::FromArgb(178, 223, 252)) ([Drawing.Color]::FromArgb(129, 190, 240))
    # 手前のモニター
    Draw-Monitor $g 92 78 156 112 ([Drawing.Color]::FromArgb(38, 50, 56)) `
        ([Drawing.Color]::FromArgb(66, 165, 245)) ([Drawing.Color]::FromArgb(21, 101, 192))

    # 円形矢印（元に戻す）
    $cx = 170; $cy = 134; $r = 30
    $w = if ($size -le 24) { 20 } elseif ($size -le 48) { 16 } else { 13 }
    $pen = New-Object Drawing.Pen ([Drawing.Color]::White), $w
    $pen.StartCap = 'Round'
    $g.DrawArc($pen, $cx - $r, $cy - $r, $r * 2, $r * 2, -20, 270)
    # 矢じり（円弧の終端 250° 付近、反時計回りの接線方向）
    $a = 250 * [Math]::PI / 180
    $ex = $cx + $r * [Math]::Cos($a); $ey = $cy + $r * [Math]::Sin($a)
    $head = $w * 1.9
    $pts = [Drawing.PointF[]]@(
        (New-Object Drawing.PointF ($ex + $head), ($ey)),
        (New-Object Drawing.PointF ($ex - $head * 0.35), ($ey - $head)),
        (New-Object Drawing.PointF ($ex - $head * 0.35), ($ey + $head))
    )
    $g.FillPolygon([Drawing.Brushes]::White, $pts)

    $g.Dispose()
    $bmp
}

# 32bit DIB 形式（BITMAPINFOHEADER + BGRA ボトムアップ + AND マスク）
function To-Dib([Drawing.Bitmap]$bmp) {
    $s = $bmp.Width
    $ms = New-Object IO.MemoryStream
    $w = New-Object IO.BinaryWriter $ms
    $w.Write([UInt32]40); $w.Write([Int32]$s); $w.Write([Int32]($s * 2))
    $w.Write([UInt16]1); $w.Write([UInt16]32); $w.Write([UInt32]0)
    $w.Write([UInt32]0); $w.Write([Int32]0); $w.Write([Int32]0); $w.Write([UInt32]0); $w.Write([UInt32]0)
    for ($y = $s - 1; $y -ge 0; $y--) {
        for ($x = 0; $x -lt $s; $x++) {
            $c = $bmp.GetPixel($x, $y)
            $w.Write([byte]$c.B); $w.Write([byte]$c.G); $w.Write([byte]$c.R); $w.Write([byte]$c.A)
        }
    }
    $maskRow = [int]([Math]::Ceiling($s / 32.0) * 4)
    $w.Write((New-Object byte[] ($maskRow * $s)))   # アルファを使うので AND マスクは全 0
    $w.Flush()
    return , $ms.ToArray()   # 配列が展開されないようにカンマで包む
}

# ICO を書き出す（256px は PNG、それ以外は互換性のため DIB）
$images = foreach ($s in $sizes) {
    $bmp = Render $s
    if ($s -ge 256) {
        $ms = New-Object IO.MemoryStream
        $bmp.Save($ms, [Drawing.Imaging.ImageFormat]::Png)
        $data = $ms.ToArray()
    } else {
        $data = To-Dib $bmp
    }
    $bmp.Dispose()
    , $data
}

$fs = [IO.File]::Create($out)
$bw = New-Object IO.BinaryWriter $fs
$bw.Write([UInt16]0); $bw.Write([UInt16]1); $bw.Write([UInt16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]; $len = $images[$i].Length
    $bw.Write([byte]($s % 256)); $bw.Write([byte]($s % 256))   # 256 は 0 で表す
    $bw.Write([byte]0); $bw.Write([byte]0)
    $bw.Write([UInt16]1); $bw.Write([UInt16]32)
    $bw.Write([UInt32]$len); $bw.Write([UInt32]$offset)
    $offset += $len
}
foreach ($img in $images) { $bw.Write([byte[]]$img) }
$bw.Close()
Write-Host "Created: $((Resolve-Path $out).Path)"
