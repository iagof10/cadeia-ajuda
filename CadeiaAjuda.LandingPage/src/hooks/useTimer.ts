import { useEffect, useRef, useState } from 'react'

export function useTimer(initialText: string) {
  const [seconds, setSeconds] = useState<number | null>(() => {
    const match = initialText.match(/(\d+)m\s*(\d+)s/)
    if (match) return parseInt(match[1]) * 60 + parseInt(match[2])
    return null
  })

  const intervalRef = useRef<number | null>(null)

  useEffect(() => {
    if (seconds === null) return

    intervalRef.current = window.setInterval(() => {
      setSeconds((prev) => (prev !== null ? prev + 1 : prev))
    }, 1000)

    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current)
    }
  }, [seconds !== null])

  if (seconds === null) return initialText

  const m = Math.floor(seconds / 60)
    .toString()
    .padStart(2, '0')
  const s = (seconds % 60).toString().padStart(2, '0')
  return `${m}m ${s}s`
}
