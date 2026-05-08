import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import toast from 'react-hot-toast'
import { usersApi } from '../../api/operations.api'

export default function SignupPage() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirm, setShowConfirm] = useState(false)

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm()

  const password = watch('password')

  const onSubmit = async (data) => {
    setLoading(true)
    try {
      await usersApi.create({
        userName: data.name.trim(),
        email:    data.email.trim().toLowerCase(),
        password: data.password,
        status:   'Active',
      })
      // Drop any stale token so /login renders the form, not a redirect target.
      localStorage.removeItem('sh-auth')
      toast.success('Account created! Please sign in.')
      navigate('/login', { replace: true })
    } catch (err) {
      const data = err.response?.data
      const firstModelError = data?.errors
        ? Object.values(data.errors).flat().find(Boolean)
        : null
      const msg = firstModelError
        ?? data?.message
        ?? (typeof data === 'string' ? data : null)
        ?? 'Registration failed'
      if (typeof msg === 'string' && msg.toLowerCase().includes('email')) {
        toast.error('This email is already registered.')
      } else {
        toast.error(msg)
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-1">Create account</h2>
      <p className="text-sm text-gray-500 mb-6">Fill in the details below to get started</p>

      <form onSubmit={handleSubmit(onSubmit)} noValidate>

        {/* Name */}
        <div className="mb-4">
          <label className="sh-label">
            Full Name <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            className={`sh-input ${errors.name ? 'border-red-400' : ''}`}
            placeholder="John Doe"
            {...register('name', {
              required: 'Full name is required',
              minLength: { value: 2, message: 'Name must be at least 2 characters' },
              maxLength: { value: 100, message: 'Name too long' },
              validate: v => v.trim().length >= 2 || 'Name cannot be blank',
            })}
          />
          {errors.name && (
            <p className="text-red-500 text-xs mt-1">⚠ {errors.name.message}</p>
          )}
        </div>

        {/* Email */}
        <div className="mb-4">
          <label className="sh-label">
            Email Address <span className="text-red-500">*</span>
          </label>
          <input
            type="email"
            className={`sh-input ${errors.email ? 'border-red-400' : ''}`}
            placeholder="you@company.com"
            {...register('email', {
              required: 'Email is required',
              pattern: {
                value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: 'Enter a valid email address',
              },
            })}
          />
          {errors.email && (
            <p className="text-red-500 text-xs mt-1">⚠ {errors.email.message}</p>
          )}
        </div>

        {/* Password */}
        <div className="mb-4">
          <label className="sh-label">
            Password <span className="text-red-500">*</span>
          </label>
          <div className="relative">
            <input
              type={showPassword ? 'text' : 'password'}
              className={`sh-input pr-10 ${errors.password ? 'border-red-400' : ''}`}
              placeholder="Min. 8 characters"
              {...register('password', {
                required: 'Password is required',
                minLength: { value: 8, message: 'Password must be at least 8 characters' },
                validate: {
                  hasUpper:  v => /[A-Z]/.test(v) || 'Must contain at least one uppercase letter',
                  hasLower:  v => /[a-z]/.test(v) || 'Must contain at least one lowercase letter',
                  hasNumber: v => /[0-9]/.test(v) || 'Must contain at least one number',
                },
              })}
            />
            <button
              type="button"
              onClick={() => setShowPassword(p => !p)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 text-xs"
              tabIndex={-1}
            >
              {showPassword ? 'Hide' : 'Show'}
            </button>
          </div>
          {errors.password && (
            <p className="text-red-500 text-xs mt-1">⚠ {errors.password.message}</p>
          )}
          <PasswordStrength password={password} />
        </div>

        {/* Confirm Password */}
        <div className="mb-6">
          <label className="sh-label">
            Confirm Password <span className="text-red-500">*</span>
          </label>
          <div className="relative">
            <input
              type={showConfirm ? 'text' : 'password'}
              className={`sh-input pr-10 ${errors.confirmPassword ? 'border-red-400' : ''}`}
              placeholder="Re-enter your password"
              {...register('confirmPassword', {
                required: 'Please confirm your password',
                validate: v => v === password || 'Passwords do not match',
              })}
            />
            <button
              type="button"
              onClick={() => setShowConfirm(p => !p)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 text-xs"
              tabIndex={-1}
            >
              {showConfirm ? 'Hide' : 'Show'}
            </button>
          </div>
          {errors.confirmPassword && (
            <p className="text-red-500 text-xs mt-1">⚠ {errors.confirmPassword.message}</p>
          )}
        </div>

        <button
          type="submit"
          disabled={loading}
          className="btn btn-primary w-full justify-center"
          style={{ background: '#1e4678' }}
        >
          {loading ? (
            <span className="flex items-center gap-2">
              <svg className="animate-spin w-4 h-4" viewBox="0 0 24 24" fill="none">
                <circle cx="12" cy="12" r="10" stroke="white" strokeWidth="3" strokeOpacity=".25"/>
                <path d="M12 2a10 10 0 0110 10" stroke="white" strokeWidth="3" strokeLinecap="round"/>
              </svg>
              Creating account…
            </span>
          ) : 'Create Account'}
        </button>

        <p className="text-center text-sm text-gray-500 mt-4">
          Already have an account?{' '}
          <Link to="/login" className="text-blue-600 font-medium hover:underline">
            Sign in
          </Link>
        </p>

      </form>
    </div>
  )
}

/* ── Password strength indicator ───────────────────────── */
function PasswordStrength({ password }) {
  if (!password) return null

  const score = [
    password.length >= 8,
    /[A-Z]/.test(password),
    /[a-z]/.test(password),
    /[0-9]/.test(password),
    /[^A-Za-z0-9]/.test(password),
  ].filter(Boolean).length

  const levels = [
    { label: 'Very weak',    color: '#ef4444', width: '20%'  },
    { label: 'Weak',         color: '#f97316', width: '40%'  },
    { label: 'Fair',         color: '#eab308', width: '60%'  },
    { label: 'Strong',       color: '#22c55e', width: '80%'  },
    { label: 'Very strong',  color: '#16a34a', width: '100%' },
  ]
  const level = levels[score - 1] ?? levels[0]

  return (
    <div className="mt-2">
      <div className="h-1.5 bg-gray-200 rounded-full overflow-hidden">
        <div
          className="h-full rounded-full transition-all duration-300"
          style={{ width: level.width, background: level.color }}
        />
      </div>
      <p className="text-xs mt-1" style={{ color: level.color }}>
        {level.label}
      </p>
    </div>
  )
}