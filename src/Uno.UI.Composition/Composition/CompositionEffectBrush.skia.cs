﻿#nullable enable

using System;
using SkiaSharp;
using Windows.UI;
using Microsoft.UI;
using System.Numerics;
using Uno.UI.Composition;
using Windows.Graphics.Effects;
using Windows.Graphics.Effects.Interop;

namespace Windows.UI.Composition
{
	public partial class CompositionEffectBrush : CompositionBrush
	{
		private bool _isCurrentInputBackdrop;

		private SKRect _currentBounds;
		private SKImageFilter? _filter;

		internal bool HasBackdropBrushInput { get; private set; }

		internal bool UseBlurPadding { get; set; }

		private SKImageFilter? GenerateEffectFilter(object? effect, SKRect bounds)
		{
			// TODO: https://user-images.githubusercontent.com/34550324/264485558-d7ee5062-b0e0-4f6e-a8c7-0620ec561d3d.png
			// TODO: Cache pixel shaders (see dwmcore.dll!CCompiledEffectCache), needed in order to implement animations and online rendering

			switch (effect)
			{
				case CompositionEffectSourceParameter effectSourceParameter:
					{
						CompositionBrush? brush = GetSourceParameter(effectSourceParameter.Name);
						if (brush is not null)
						{
							if (brush is CompositionBackdropBrush)
							{
								_isCurrentInputBackdrop = true;
								HasBackdropBrushInput = true;
								return null;
							}

							_isCurrentInputBackdrop = false;

							SKPaint paint = new SKPaint() { IsAntialias = true, IsAutohinted = true, FilterQuality = SKFilterQuality.High };
							brush.UpdatePaint(paint, bounds);

							return SKImageFilter.CreatePaint(paint, new(bounds));
						}

						return null;
					}
				case IGraphicsEffectD2D1Interop effectInterop:
					{
						switch (EffectHelpers.GetEffectType(effectInterop.GetEffectId()))
						{
							case EffectType.GaussianBlurEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 3 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("BlurAmount", out uint sigmaProp, out _);
										effectInterop.GetNamedPropertyMapping("Optimization", out uint optProp, out _);
										effectInterop.GetNamedPropertyMapping("BorderMode", out uint borderProp, out _);

										float sigma = (float)(effectInterop.GetProperty(sigmaProp) ?? throw new NullReferenceException("The effect property was null"));
										_ = (uint?)effectInterop.GetProperty(optProp); // TODO
										_ = (uint?)effectInterop.GetProperty(borderProp); // TODO

										return SKImageFilter.CreateBlur(sigma, sigma, sourceFilter, new(UseBlurPadding ? bounds with { Left = -100, Top = -100, Right = bounds.Right + 100, Bottom = bounds.Bottom + 100 } : bounds));
									}

									return null;
								}
							case EffectType.GrayscaleEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Grayscale Matrix
												{
													0.21f, 0.72f, 0.07f, 0, 0,
													0.21f, 0.72f, 0.07f, 0, 0,
													0.21f, 0.72f, 0.07f, 0, 0,
													0,     0,     0,     1, 0
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.InvertEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Invert Matrix
												{
													-1, 0,  0,  0, 1,
													0,  -1, 0,  0, 1,
													0,  0,  -1, 0, 1,
													0,  0,  0,  1, 0,
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.HueRotationEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Angle", out uint angleProp, out GraphicsEffectPropertyMapping angleMapping);
										float angle = (float)(effectInterop.GetProperty(angleProp) ?? throw new NullReferenceException("The effect property was null"));

										if (angleMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											angle *= 180.0f / MathF.PI;
										}

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Hue Rotation Matrix
												{
													0.2127f + MathF.Cos(angle) * 0.7873f - MathF.Sin(angle) * 0.2127f, 0.715f - MathF.Cos(angle) * 0.715f - MathF.Sin(angle) * 0.715f, 0.072f - MathF.Cos(angle) * 0.072f + MathF.Sin(angle) * 0.928f, 0, 0,
													0.2127f - MathF.Cos(angle) * 0.213f + MathF.Sin(angle) * 0.143f,   0.715f + MathF.Cos(angle) * 0.285f + MathF.Sin(angle) * 0.140f, 0.072f - MathF.Cos(angle) * 0.072f - MathF.Sin(angle) * 0.283f, 0, 0,
													0.2127f - MathF.Cos(angle) * 0.213f - MathF.Sin(angle) * 0.787f,   0.715f - MathF.Cos(angle) * 0.715f + MathF.Sin(angle) * 0.715f, 0.072f + MathF.Cos(angle) * 0.928f + MathF.Sin(angle) * 0.072f, 0, 0,
													0,                                                                   0,                                                                0,                                                                1, 0
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.TintEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 1 /* only the Color property is required */ && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										// Note: ColorHdr isn't supported by Composition (as of 10.0.25941.1000)
										effectInterop.GetNamedPropertyMapping("Color", out uint colorProp, out _);
										effectInterop.GetNamedPropertyMapping("ClampOutput", out uint clampProp, out _);

										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));
										bool clamp = clampProp != 0xFF ? (bool?)effectInterop.GetProperty(clampProp) ?? false : false;

										string shader = $@"
											uniform shader input;
											uniform vec4 color;

											half4 main() 
											{{
												return {(clamp ? "clamp(" : String.Empty)}sample(input) * color{(clamp ? ", 0.0, 1.0)" : String.Empty)};
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "color", new float[] { color.R * (1.0f / 255.0f), color.G * (1.0f / 255.0f), color.B * (1.0f / 255.0f), color.A * (1.0f / 255.0f) } }
										};
										SKRuntimeEffectChildren children = new(runtimeEffect)
										{
											{ "input", null }
										};

										return SKImageFilter.CreateColorFilter(runtimeEffect.ToColorFilter(uniforms, children), sourceFilter, new(bounds));

										// Reference (wuceffects.dll):
										/*
											void Windows::UI::Composition::TintEffectType::GenerateCode(const Windows::UI::Composition::EffectNode *node, Windows::UI::Composition::EffectGenerator *pGenerator, const char *pszOutputPixelName)
											{
												Windows::UI::Composition::StringBuilder *pStringBuilder;
												std::string strInputPixel;
												std::string strColorProperty;

												strInputPixel = pGenerator->GetInputPixelName(node, 0);
												strColorProperty = pGenerator->DeclareShaderVariableForProperty(0); // Color
  
												pStringBuilder = pGenerator->BeginPSLine();
												pStringBuilder->Append("    ");
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = ");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(" * ");
												pStringBuilder->Append(strColorProperty.c_str(), strColorProperty.size());
												pStringBuilder->Append(";");
												pStringBuilder->Append('\n');
  
												if (*(bool*)&node->m_uprgbDefaultProperties[16]) // ClampOutput, 16 = GetPropertyMetadata(1, &metatdata) ==> metatdata.cbStructOffset
												{
													Windows::UI::Composition::StringBuilder* builder = pGenerator->BeginPSLine();
													builder->Append(pszOutputPixelName);
													builder->Append(" = saturate(");
													builder->Append(pszOutputPixelName);
													builder->Append(");");
													builder->Append('\n');
												}
											}
										*/
									}

									return null;
								}
							case EffectType.BlendEffect: // TODO: Replace this with a pixel shader to get the same output as Windows
								{
									if (effectInterop.GetSourceCount() == 2 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource bg && effectInterop.GetSource(1) is IGraphicsEffectSource fg)
									{
										SKImageFilter? bgFilter = GenerateEffectFilter(bg, bounds);
										if (bgFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										SKImageFilter? fgFilter = GenerateEffectFilter(fg, bounds);
										if (fgFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Mode", out uint modeProp, out _);
										D2D1BlendEffectMode mode = (D2D1BlendEffectMode)(effectInterop.GetProperty(modeProp) ?? throw new NullReferenceException("The effect property was null"));
										SKBlendMode skMode = mode.ToSkia();

										if (skMode == (SKBlendMode)0xFF) // Unsupported mode, fallback to default mode, we can add support for other modes when we move to Skia 3 through pixel shaders
										{
											skMode = SKBlendMode.Multiply;
										}

										return SKImageFilter.CreateBlendMode(skMode, bgFilter, fgFilter, new(bounds));
									}

									return null;
								}
							case EffectType.CompositeEffect:
								{
									if (effectInterop.GetSourceCount() > 1 && effectInterop.GetPropertyCount() == 1)
									{
										SKImageFilter? currentFilter = GenerateEffectFilter(effectInterop.GetSource(0), bounds);
										if (currentFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Mode", out uint modeProp, out _);
										D2D1CompositeMode mode = (D2D1CompositeMode)(effectInterop.GetProperty(modeProp) ?? throw new NullReferenceException("The effect property was null"));
										SKBlendMode skMode = mode.ToSkia();

										if (skMode == (SKBlendMode)0xFF) // Unsupported mode, fallback to default mode, we can add support for other modes when we move to Skia 3 through pixel shaders
										{
											skMode = SKBlendMode.SrcOver;
										}

										// We have to do this manually because SKImageFilter.CreateMerge(SKImageFilter, SKImageFilter, SKBlendMode, SKImageFilter.CropRect) is obsolete.
										for (uint idx = 1; idx < effectInterop.GetSourceCount(); idx++)
										{
											SKImageFilter? nextFilter = GenerateEffectFilter(effectInterop.GetSource(idx), bounds);

											if (nextFilter is not null && !_isCurrentInputBackdrop)
											{
												currentFilter = SKImageFilter.CreateBlendMode(skMode, currentFilter, nextFilter, new(bounds));
											}

											_isCurrentInputBackdrop = false;
										}

										return currentFilter;
									}

									return null;
								}
							case EffectType.ColorSourceEffect:
								{
									if (effectInterop.GetPropertyCount() >= 1 /* only the Color property is required */)
									{
										// Note: ColorHdr isn't supported by Composition (as of 10.0.25941.1000)
										effectInterop.GetNamedPropertyMapping("Color", out uint colorProp, out _);
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										return SKImageFilter.CreatePaint(new SKPaint() { Color = color.ToSKColor() }, new(bounds));
									}

									return null;
								}
							case EffectType.OpacityEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Opacity", out uint opacityProp, out _);
										float opacity = (float)(effectInterop.GetProperty(opacityProp) ?? throw new NullReferenceException("The effect property was null"));


										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Opacity Matrix
												{
													1, 0, 0, 0,       0,
													0, 1, 0, 0,       0,
													0, 0, 1, 0,       0,
													0, 0, 0, opacity, 0
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.ContrastEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 1 /* only the Contrast property is required */ && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Contrast", out uint contrastProp, out _);
										effectInterop.GetNamedPropertyMapping("ClampSource", out uint clampProp, out _);

										float contrast = (float)(effectInterop.GetProperty(contrastProp) ?? throw new NullReferenceException("The effect property was null"));
										bool clamp = clampProp != 0xFF ? (bool?)effectInterop.GetProperty(clampProp) ?? false : false;

										string shader = $@"
											uniform shader input;
											uniform half contrastValue;

											half4 Premultiply(half4 color)
											{{
												color.rgb *= color.a;
												return color;
											}}

											half4 UnPremultiply(half4 color)
											{{
												color.rgb = (color.a == 0) ? half3(0, 0, 0) : (color.rgb / color.a);
												return color;
											}}

											half4 Contrast(half4 color, half contrast)
											{{
												color = UnPremultiply(color);

												half s = 1 - (3.0 / 4.0) * contrast;
												half c2 = s - 1;
												half b2 = 4 - 3 * s;
												half a2 = 2 * c2;
												half b1 = s;
												half a1 = -a2;
    
												half3 lowResult = color.rgb * (color.rgb * a1 + b1);
												half3 highResult = color.rgb * (color.rgb * a2 + b2) + c2;
    
												half3 comparisonResult = half3(0.0);
												comparisonResult.r = (color.rgb.r < 0.5) ? 1.0 : 0.0;
												comparisonResult.g = (color.rgb.g < 0.5) ? 1.0 : 0.0;
												comparisonResult.b = (color.rgb.b < 0.5) ? 1.0 : 0.0;

												color.rgb = mix(lowResult, highResult, comparisonResult);
    
												return Premultiply(color);
											}}

											half4 main() 
											{{
												return Contrast({(clamp ? "clamp(" : String.Empty)}sample(input){(clamp ? ", 0.0, 1.0)" : String.Empty)}, contrastValue);
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "contrastValue", contrast }
										};
										SKRuntimeEffectChildren children = new(runtimeEffect)
										{
											{ "input", null }
										};

										return SKImageFilter.CreateColorFilter(runtimeEffect.ToColorFilter(uniforms, children), sourceFilter, new(bounds));

										// Reference (wuceffects.dll):
										/*
											void Windows::UI::Composition::ContrastEffectType::GenerateCode(const Windows::UI::Composition::EffectNode *node, Windows::UI::Composition::EffectGenerator *pGenerator, const char *pszOutputPixelName)
											{
												Windows::UI::Composition::StringBuilder *pStringBuilder;
												std::string strInputPixel;
												std::string strContrastProperty;

												strInputPixel = pGenerator->GetInputPixelName(node, 0);
												strContrastProperty = pGenerator->DeclareShaderVariableForProperty(0); // Contrast
  
												pGenerator->AddPSInclude("D2DContrast.hlsl");
												pStringBuilder = pGenerator->BeginPSLine();
												pStringBuilder->Append("    ");
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = ");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(";");
												pStringBuilder->Append('\n');
  
												if (*(bool*)&node->m_uprgbDefaultProperties[4]) // ClampSource, 4 = GetPropertyMetadata(1, &metatdata) ==> metatdata.cbStructOffset
												{
													Windows::UI::Composition::StringBuilder* builder = pGenerator->BeginPSLine();
													builder->Append(pszOutputPixelName);
													builder->Append(" = saturate(");
													builder->Append(pszOutputPixelName);
													builder->Append(");");
													builder->Append('\n');
												}

												Windows::UI::Composition::StringBuilder* builder = pGenerator->BeginPSLine();
												builder->Append(pszOutputPixelName);
												builder->Append(" = D2DContrast(");
												builder->Append(pszOutputPixelName);
												builder->Append(", ");
												builder->Append(strContrastProperty.c_str(), strContrastProperty.size());
												builder->Append(");");
												builder->Append('\n');
											}
										*/
									}

									return null;
								}
							case EffectType.ArithmeticCompositeEffect: // TODO: support "ClampOutput" property
								{
									if (effectInterop.GetSourceCount() == 2 && effectInterop.GetPropertyCount() >= 4 && effectInterop.GetSource(0) is IGraphicsEffectSource bg && effectInterop.GetSource(1) is IGraphicsEffectSource fg)
									{
										SKImageFilter? bgFilter = GenerateEffectFilter(bg, bounds);
										if (bgFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										SKImageFilter? fgFilter = GenerateEffectFilter(fg, bounds);
										if (fgFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										float? multiplyAmount = effectInterop.GetProperty(0) as float?;
										float? source1Amount = effectInterop.GetProperty(1) as float?;
										float? source2Amount = effectInterop.GetProperty(2) as float?;
										float? offset = effectInterop.GetProperty(3) as float?;

										if (multiplyAmount is null || source1Amount is null || source2Amount is null || offset is null)
										{
											float[]? coefficients = effectInterop.GetProperty(0) as float[];

											if (coefficients is null)
											{
												effectInterop.GetNamedPropertyMapping("Coefficients", out uint coefficientsProp, out GraphicsEffectPropertyMapping coefficientsMapping);
												if (coefficientsMapping == GraphicsEffectPropertyMapping.Direct)
												{
													coefficients = effectInterop.GetProperty(coefficientsProp) as float[];
												}
												else
												{
													return null;
												}
											}

											if (coefficients is not null && coefficients.Length == 4)
											{
												multiplyAmount = coefficients[0];
												source1Amount = coefficients[1];
												source2Amount = coefficients[2];
												offset = coefficients[3];
											}
											else
											{
												return null;
											}
										}

										return SKImageFilter.CreateArithmetic(multiplyAmount.Value, source1Amount.Value, source2Amount.Value, offset.Value, false, bgFilter, fgFilter, new(bounds));
									}

									return null;
								}
							case EffectType.ExposureEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Exposure", out uint exposureProp, out _);

										float exposure = (float)(effectInterop.GetProperty(exposureProp) ?? throw new NullReferenceException("The effect property was null"));
										float multiplier = MathF.Pow(2.0f, exposure);

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Exposure Matrix
												{
													multiplier, 0,          0,          0, 0,
													0,          multiplier, 0,          0, 0,
													0,          0,          multiplier, 0, 0,
													0,          0,          0,          1, 0,
												}),
											sourceFilter, new(bounds));

										// Reference (wuceffects.dll):
										/*
											void Windows::UI::Composition::ExposureEffectType::GenerateCode(const Windows::UI::Composition::EffectNode *node, Windows::UI::Composition::EffectGenerator *pGenerator, const char *pszOutputPixelName)
											{
												Windows::UI::Composition::StringBuilder *pStringBuilder;
												std::string strInputPixel;
												std::string strMultiplierProperty;
												bool isPropertyDynamic;

												strInputPixel = pGenerator->GetInputPixelName(node, 0);

												isPropertyDynamic = node->IsPropertyDynamic(0); // ExposureValue
												pGenerator->DeclareShaderVariable(
													&strMultiplierProperty,
													DCOMPOSITION_EXPRESSION_TYPE_SCALAR,
													"Multiplier",
													isPropertyDynamic,
													[](void** defaultProperties, void** output) { *(float*)*output = pow(2.0, *(float*)(defaultProperties[0])); }
												);
  
												pStringBuilder = pGenerator->BeginPSLine();
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = minfloat4(");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(".rgb * ");
												pStringBuilder->Append(strMultiplierProperty.c_str(), strMultiplierProperty.size());
												pStringBuilder->Append(", ");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(".a);");
												pStringBuilder->Append('\n');
											}
										*/
									}

									return null;
								}
							case EffectType.CrossFadeEffect: // TODO: We should use SkColorFilters::Lerp instead once SkiaSharp includes it
								{
									if (effectInterop.GetSourceCount() == 2 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource sourceA && effectInterop.GetSource(1) is IGraphicsEffectSource sourceB)
									{
										SKImageFilter? sourceFilter1 = GenerateEffectFilter(sourceB, bounds);
										if (sourceFilter1 is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										SKImageFilter? sourceFilter2 = GenerateEffectFilter(sourceA, bounds);
										if (sourceFilter2 is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("CrossFade", out uint crossfadeProp, out _);

										float crossfade = (float)(effectInterop.GetProperty(crossfadeProp) ?? throw new NullReferenceException("The effect property was null"));

										if (crossfade <= 0.0f)
										{
											return sourceFilter1;
										}
										else if (crossfade >= 1.0f)
										{
											return sourceFilter2;
										}

										SKImageFilter fbFilter = SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[]
												{
													crossfade, 0,         0,         0,         0,
													0,         crossfade, 0,         0,         0,
													0,         0,         crossfade, 0,         0,
													0,         0,         0,         crossfade, 0
												}),
											sourceFilter2);

										string shader = $@"
											uniform shader input;
											uniform half crossfade;

											half4 main() 
											{{
												half4 inputColor = sample(input);
												return inputColor - (inputColor * crossfade);
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "crossfade", crossfade }
										};
										SKRuntimeEffectChildren children = new(runtimeEffect)
										{
											{ "input", null }
										};

										SKImageFilter amafFilter = SKImageFilter.CreateColorFilter(runtimeEffect.ToColorFilter(uniforms, children), sourceFilter1);

										return SKImageFilter.CreateBlendMode(SKBlendMode.Plus, fbFilter, amafFilter, new(bounds));
									}

									return null;
								}
							case EffectType.LuminanceToAlphaEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										return SKImageFilter.CreateColorFilter(SKColorFilter.CreateLumaColor(), sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.LinearTransferEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 13 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("RedOffset", out uint redOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("RedSlope", out uint redSlopeProp, out _);
										effectInterop.GetNamedPropertyMapping("RedDisable", out uint redDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("GreenOffset", out uint greenOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("GreenSlope", out uint greenSlopeProp, out _);
										effectInterop.GetNamedPropertyMapping("GreenDisable", out uint greenDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("BlueOffset", out uint blueOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("BlueSlope", out uint blueSlopeProp, out _);
										effectInterop.GetNamedPropertyMapping("BlueDisable", out uint blueDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("AlphaOffset", out uint alphaOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("AlphaSlope", out uint alphaSlopeProp, out _);
										effectInterop.GetNamedPropertyMapping("AlphaDisable", out uint alphaDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("ClampOutput", out uint clampProp, out _);

										float redOffset = (float)(effectInterop.GetProperty(redOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										float redSlope = (float)(effectInterop.GetProperty(redSlopeProp) ?? throw new NullReferenceException("The effect property was null"));
										bool redDisable = (bool)(effectInterop.GetProperty(redDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float greenOffset = (float)(effectInterop.GetProperty(greenOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										float greenSlope = (float)(effectInterop.GetProperty(greenSlopeProp) ?? throw new NullReferenceException("The effect property was null"));
										bool greenDisable = (bool)(effectInterop.GetProperty(greenDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float blueOffset = (float)(effectInterop.GetProperty(blueOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										float blueSlope = (float)(effectInterop.GetProperty(blueSlopeProp) ?? throw new NullReferenceException("The effect property was null"));
										bool blueDisable = (bool)(effectInterop.GetProperty(blueDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float alphaOffset = (float)(effectInterop.GetProperty(alphaOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										float alphaSlope = (float)(effectInterop.GetProperty(alphaSlopeProp) ?? throw new NullReferenceException("The effect property was null"));
										bool alphaDisable = (bool)(effectInterop.GetProperty(alphaDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										bool clamp = clampProp != 0xFF ? (bool?)effectInterop.GetProperty(clampProp) ?? false : false;

										string shader = $@"
											uniform shader input;

											uniform half redOffset;
											uniform half redSlope;

											uniform half greenOffset;
											uniform half greenSlope;

											uniform half blueOffset;
											uniform half blueSlope;

											uniform half alphaOffset;
											uniform half alphaSlope;

											half4 Premultiply(half4 color)
											{{
												color.rgb *= color.a;
												return color;
											}}

											half4 UnPremultiply(half4 color)
											{{
												color.rgb = (color.a == 0) ? half3(0, 0, 0) : (color.rgb / color.a);
												return color;
											}}

											half4 main()
											{{
												half4 color = UnPremultiply(sample(input));
												color = half4(
													{(redDisable ? "color.r" : "redOffset + color.r * redSlope")},
													{(greenDisable ? "color.g" : "greenOffset + color.g * greenSlope")},
													{(blueDisable ? "color.b" : "blueOffset + color.b * blueSlope")},
													{(alphaDisable ? "color.a" : "alphaOffset + color.a * alphaSlope")}
												);

												return {(clamp ? "clamp(" : String.Empty)}Premultiply(color){(clamp ? ", 0.0, 1.0)" : String.Empty)};
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "redOffset", redOffset },
											{ "redSlope", redSlope },

											{ "greenOffset", greenOffset },
											{ "greenSlope", greenSlope },

											{ "blueOffset", blueOffset },
											{ "blueSlope", blueSlope },

											{ "alphaOffset", alphaOffset },
											{ "alphaSlope", alphaSlope }
										};
										SKRuntimeEffectChildren children = new(runtimeEffect)
										{
											{ "input", null }
										};

										return SKImageFilter.CreateColorFilter(runtimeEffect.ToColorFilter(uniforms, children), sourceFilter, new(bounds));

										// Reference (wuceffects.dll):
										/*
											void Windows::UI::Composition::LinearTransferEffectType::GenerateCode(const Windows::UI::Composition::EffectNode *node, Windows::UI::Composition::EffectGenerator *pGenerator, const char *pszOutputPixelName)
											{
												bool rgfDisable[4];
												std::string strInputPixel;
												std::string rgstrPropertyNames[8];
												Windows::UI::Composition::StringBuilder *pStringBuilder;
	
												strInputPixel = pGenerator->GetInputPixelName(node, 0);
	
												for ( int i,k = 0; i < 12; i += 3, k += 2 )
												{
													rgstrPropertyNames[k] = pGenerator->DeclareShaderVariableForProperty(i); // Offset
													rgstrPropertyNames[k + 1] = pGenerator->DeclareShaderVariableForProperty(i + 1); // Slope
												}
	
												pStringBuilder = pGenerator->BeginPSLine();
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = UnPremultiply(");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = minfloat4(");
												pStringBuilder->Append('\n');
	
												rgfDisable[0] = *(bool*)&node->m_uprgbDefaultProperties[32]; // RedDisable
												rgfDisable[1] = *(bool*)&node->m_uprgbDefaultProperties[33]; // GreenDisable
												rgfDisable[2] = *(bool*)&node->m_uprgbDefaultProperties[34]; // BlueDisable
												rgfDisable[3] = *(bool*)&node->m_uprgbDefaultProperties[35]; // AlphaDisable
	
												const char* RGBA = "rgba";
												for ( int i,k = 0; i < 4; i++, k += 2 )
												{
													if (i)
													{
														pStringBuilder->Append(",");
														pStringBuilder->Append('\n');
													}
		
													if ( rgfDisable[i] )
													{
														pStringBuilder->Append(pszOutputPixelName);
														pStringBuilder->Append('.');
														pStringBuilder->Append(RGBA[i]);
													}
													else
													{
														pStringBuilder->Append(rgstrPropertyNames[k]); // Offset
														pStringBuilder->Append(" + ");
														pStringBuilder->Append(pszOutputPixelName);
														pStringBuilder->Append('.');
														pStringBuilder->Append(RGBA[i]);
														pStringBuilder->Append(" * ");
														pStringBuilder->Append(rgstrPropertyNames[k + 1]); // Slope
													}
												}
	
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = Premultiply(");
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												if (*(bool*)&node->m_uprgbDefaultProperties[36]) // ClampOutput
												{
													pStringBuilder->Append(pszOutputPixelName);
													pStringBuilder->Append(" = saturate(");
													pStringBuilder->Append(pszOutputPixelName);
													pStringBuilder->Append(");");
													pStringBuilder->Append('\n');
												}
											}
										*/
									}

									return null;
								}
							case EffectType.GammaTransferEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 17 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("RedAmplitude", out uint redAmplitudeProp, out _);
										effectInterop.GetNamedPropertyMapping("RedExponent", out uint redExponentProp, out _);
										effectInterop.GetNamedPropertyMapping("RedOffset", out uint redOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("RedDisable", out uint redDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("GreenAmplitude", out uint greenAmplitudeProp, out _);
										effectInterop.GetNamedPropertyMapping("GreenExponent", out uint greenExponentProp, out _);
										effectInterop.GetNamedPropertyMapping("GreenOffset", out uint greenOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("GreenDisable", out uint greenDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("BlueAmplitude", out uint blueAmplitudeProp, out _);
										effectInterop.GetNamedPropertyMapping("BlueExponent", out uint blueExponentProp, out _);
										effectInterop.GetNamedPropertyMapping("BlueOffset", out uint blueOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("BlueDisable", out uint blueDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("AlphaAmplitude", out uint alphaAmplitudeProp, out _);
										effectInterop.GetNamedPropertyMapping("AlphaExponent", out uint alphaExponentProp, out _);
										effectInterop.GetNamedPropertyMapping("AlphaOffset", out uint alphaOffsetProp, out _);
										effectInterop.GetNamedPropertyMapping("AlphaDisable", out uint alphaDisableProp, out _);

										effectInterop.GetNamedPropertyMapping("ClampOutput", out uint clampProp, out _);

										float redAmplitude = (float)(effectInterop.GetProperty(redAmplitudeProp) ?? throw new NullReferenceException("The effect property was null"));
										float redExponent = (float)(effectInterop.GetProperty(redExponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float redOffset = (float)(effectInterop.GetProperty(redOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										bool redDisable = (bool)(effectInterop.GetProperty(redDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float greenAmplitude = (float)(effectInterop.GetProperty(greenAmplitudeProp) ?? throw new NullReferenceException("The effect property was null"));
										float greenExponent = (float)(effectInterop.GetProperty(greenExponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float greenOffset = (float)(effectInterop.GetProperty(greenOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										bool greenDisable = (bool)(effectInterop.GetProperty(greenDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float blueAmplitude = (float)(effectInterop.GetProperty(blueAmplitudeProp) ?? throw new NullReferenceException("The effect property was null"));
										float blueExponent = (float)(effectInterop.GetProperty(blueExponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float blueOffset = (float)(effectInterop.GetProperty(blueOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										bool blueDisable = (bool)(effectInterop.GetProperty(blueDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										float alphaAmplitude = (float)(effectInterop.GetProperty(alphaAmplitudeProp) ?? throw new NullReferenceException("The effect property was null"));
										float alphaExponent = (float)(effectInterop.GetProperty(alphaExponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float alphaOffset = (float)(effectInterop.GetProperty(alphaOffsetProp) ?? throw new NullReferenceException("The effect property was null"));
										bool alphaDisable = (bool)(effectInterop.GetProperty(alphaDisableProp) ?? throw new NullReferenceException("The effect property was null"));

										bool clamp = clampProp != 0xFF ? (bool?)effectInterop.GetProperty(clampProp) ?? false : false;

										string shader = $@"
											uniform shader input;

											uniform half redAmplitude;
											uniform half redExponent;
											uniform half redOffset;

											uniform half greenAmplitude;
											uniform half greenExponent;
											uniform half greenOffset;

											uniform half blueAmplitude;
											uniform half blueExponent;
											uniform half blueOffset;

											uniform half alphaAmplitude;
											uniform half alphaExponent;
											uniform half alphaOffset;

											half4 Premultiply(half4 color)
											{{
												color.rgb *= color.a;
												return color;
											}}

											half4 UnPremultiply(half4 color)
											{{
												color.rgb = (color.a == 0) ? half3(0, 0, 0) : (color.rgb / color.a);
												return color;
											}}

											half4 main()
											{{
												half4 color = UnPremultiply(sample(input));
												color = half4(
													{(redDisable ? "color.r" : "redAmplitude * pow(abs(color.r), redExponent) + redOffset")},
													{(greenDisable ? "color.g" : "greenAmplitude * pow(abs(color.g), greenExponent) + greenOffset")},
													{(blueDisable ? "color.b" : "blueAmplitude * pow(abs(color.b), blueExponent) + blueOffset")},
													{(alphaDisable ? "color.a" : "alphaAmplitude * pow(abs(color.a), alphaExponent) + alphaOffset")}
												);

												return {(clamp ? "clamp(" : String.Empty)}Premultiply(color){(clamp ? ", 0.0, 1.0)" : String.Empty)};
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "redAmplitude", redAmplitude },
											{ "redExponent", redExponent },
											{ "redOffset", redOffset },

											{ "greenAmplitude", greenAmplitude },
											{ "greenExponent", greenExponent },
											{ "greenOffset", greenOffset },

											{ "blueAmplitude", blueAmplitude },
											{ "blueExponent", blueExponent },
											{ "blueOffset", blueOffset },

											{ "alphaAmplitude", alphaAmplitude },
											{ "alphaExponent", alphaExponent },
											{ "alphaOffset", alphaOffset }
										};
										SKRuntimeEffectChildren children = new(runtimeEffect)
										{
											{ "input", null }
										};

										return SKImageFilter.CreateColorFilter(runtimeEffect.ToColorFilter(uniforms, children), sourceFilter, new(bounds));

										// Reference (wuceffects.dll):
										/*
											void Windows::UI::Composition::GammaTransferEffectType::GenerateCode(const Windows::UI::Composition::EffectNode *node, Windows::UI::Composition::EffectGenerator *pGenerator, const char *pszOutputPixelName)
											{
												bool rgfDisable[4];
												std::string strInputPixel;
												std::string rgstrPropertyNames[12];
												Windows::UI::Composition::StringBuilder *pStringBuilder;
	
												strInputPixel = pGenerator->GetInputPixelName(node, 0);
	
												for ( int i = 2, k = 0; i < 15; i += 4, k += 3 )
												{
													rgstrPropertyNames[k] = pGenerator->DeclareShaderVariableForProperty(i - 1); // Amplitude
													rgstrPropertyNames[k + 1] = pGenerator->DeclareShaderVariableForProperty(i); // Exponent
													rgstrPropertyNames[k + 2] = pGenerator->DeclareShaderVariableForProperty(i + 1); // Offset
												}
	
												pStringBuilder = pGenerator->BeginPSLine();
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = UnPremultiply(");
												pStringBuilder->Append(strInputPixel.c_str(), strInputPixel.size());
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = minfloat4(");
												pStringBuilder->Append('\n');
	
												rgfDisable[0] = *(bool*)&node->m_uprgbDefaultProperties[48]; // RedDisable
												rgfDisable[1] = *(bool*)&node->m_uprgbDefaultProperties[49]; // GreenDisable
												rgfDisable[2] = *(bool*)&node->m_uprgbDefaultProperties[50]; // BlueDisable
												rgfDisable[3] = *(bool*)&node->m_uprgbDefaultProperties[51]; // AlphaDisable
	
												const char* RGBA = "rgba";
												for ( int i,k = 0; i < 4; i++, k += 3 )
												{
													if (i)
													{
														pStringBuilder->Append(",");
														pStringBuilder->Append('\n');
													}
		
													if ( rgfDisable[i] )
													{
														pStringBuilder->Append(pszOutputPixelName);
														pStringBuilder->Append('.');
														pStringBuilder->Append(RGBA[i]);
													}
													else
													{
														pStringBuilder->Append(rgstrPropertyNames[k]); // Amplitude
														pStringBuilder->Append(" * pow(");
														pStringBuilder->Append("abs(");
														pStringBuilder->Append(pszOutputPixelName);
														pStringBuilder->Append('.');
														pStringBuilder->Append(RGBA[i]);
														pStringBuilder->Append(')');
														pStringBuilder->Append(", ");
														pStringBuilder->Append(rgstrPropertyNames[k + 1]); // Exponent
														pStringBuilder->Append(") + ");
														pStringBuilder->Append(rgstrPropertyNames[k + 2]); // Offset
													}
												}
	
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(" = Premultiply(");
												pStringBuilder->Append(pszOutputPixelName);
												pStringBuilder->Append(");");
												pStringBuilder->Append('\n');
	
												if (*(bool*)&node->m_uprgbDefaultProperties[52]) // ClampOutput
												{
													pStringBuilder->Append(pszOutputPixelName);
													pStringBuilder->Append(" = saturate(");
													pStringBuilder->Append(pszOutputPixelName);
													pStringBuilder->Append(");");
													pStringBuilder->Append('\n');
												}
											}
										*/
									}

									return null;
								}
							case EffectType.Transform2DEffect: // TODO: support "InterpolationMode", "BorderMode", and "Sharpness" properties
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 4 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("TransformMatrix", out uint matrixProp, out _);
										Matrix3x2? matrix = effectInterop.GetProperty(matrixProp) as Matrix3x2?;

										if (matrix is null)
										{
											float[]? matrixArray = effectInterop.GetProperty(matrixProp) as float[];

											if (matrixArray is not null && matrixArray.Length == 6)
											{
												matrix = new Matrix3x2(matrixArray[0], matrixArray[1], matrixArray[2], matrixArray[3], matrixArray[4], matrixArray[5]);
											}
											else
											{
												return null;
											}
										}

										return SKImageFilter.CreateMerge(new[] { SKImageFilter.CreateMatrix(matrix.Value.ToSKMatrix(), SKFilterQuality.High, sourceFilter) }, new(bounds));
									}

									return null;
								}
							case EffectType.BorderEffect: // Note: Clamp and Mirror are currently broken, see https://github.com/mono/SkiaSharp/issues/866
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 2 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("ExtendX", out uint modeXProp, out _);
										effectInterop.GetNamedPropertyMapping("ExtendY", out uint modeYProp, out _);
										D2D1BorderEdgeMode xmode = (D2D1BorderEdgeMode)(effectInterop.GetProperty(modeXProp) ?? throw new NullReferenceException("The effect property was null"));
										D2D1BorderEdgeMode ymode = (D2D1BorderEdgeMode)(effectInterop.GetProperty(modeYProp) ?? throw new NullReferenceException("The effect property was null"));

										SKShaderTileMode mode;

										// TODO: support separate X,Y modes
										if (xmode != ymode)
										{
											if (xmode != default)
											{
												mode = xmode.ToSkia();
											}
											else
											{
												mode = ymode.ToSkia();
											}
										}
										else
										{
											mode = xmode.ToSkia();
										}

										float[] identityKernel = new float[9]
										{
											0,0,0,
											0,1,0,
											0,0,0
										};

										return SKImageFilter.CreateMatrixConvolution(new SKSizeI(3, 3), identityKernel, 1f, 0f, new(0, 0), mode, true, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.SepiaEffect: // TODO: support "AlphaMode" property maybe?
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Intensity", out uint intensityProp, out _);
										float intensity = (float)(effectInterop.GetProperty(intensityProp) ?? throw new NullReferenceException("The effect property was null"));


										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Sepia Matrix
												{
													0.393f + 0.607f * (1 - intensity), 0.769f - 0.769f * (1 - intensity), 0.189f - 0.189f * (1 - intensity), 0, 0,
													0.349f - 0.349f * (1 - intensity), 0.686f + 0.314f * (1 - intensity), 0.168f - 0.168f * (1 - intensity), 0, 0,
													0.272f - 0.272f * (1 - intensity), 0.534f - 0.534f * (1 - intensity), 0.131f + 0.869f * (1 - intensity), 0, 0,
													0,                                 0,                                 0,                                 1, 0
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.TemperatureAndTintEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 2 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Temperature", out uint tempProp, out _);
										effectInterop.GetNamedPropertyMapping("Tint", out uint tintProp, out _);

										float temp = (float)(effectInterop.GetProperty(tempProp) ?? throw new NullReferenceException("The effect property was null"));
										float tint = (float)(effectInterop.GetProperty(tintProp) ?? throw new NullReferenceException("The effect property was null"));

										var gains = TempAndTintUtils.NormalizedTempTintToGains(temp, tint);

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // TemperatureAndTint Matrix
												{
													gains.RedGain, 0,          0,              0, 0,
													0,             1,          0,              0, 0,
													0,             0,          gains.BlueGain, 0, 0,
													0,             0,          0,              1, 0,
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.ColorMatrixEffect: // TODO: support "AlphaMode" and "ClampOutput" properties
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("ColorMatrix", out uint matrixProp, out _);
										float[] matrix = (float[])(effectInterop.GetProperty(matrixProp) ?? throw new NullReferenceException("The effect property was null"));

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[]
												{
													matrix[0],  matrix[1],  matrix[2],  matrix[3],  matrix[16],
													matrix[4],  matrix[5],  matrix[6],  matrix[7],  matrix[17],
													matrix[8],  matrix[9],  matrix[10], matrix[11], matrix[18],
													matrix[12], matrix[13], matrix[14], matrix[15], matrix[19],
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.DistantDiffuseEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 4 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Azimuth", out uint azimuthProp, out GraphicsEffectPropertyMapping azimuthMapping);
										effectInterop.GetNamedPropertyMapping("Elevation", out uint elevationProp, out GraphicsEffectPropertyMapping elevationMapping);
										effectInterop.GetNamedPropertyMapping("DiffuseAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										float azimuth = (float)(effectInterop.GetProperty(azimuthProp) ?? throw new NullReferenceException("The effect property was null"));
										float elevation = (float)(effectInterop.GetProperty(elevationProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										if (azimuthMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											azimuth *= 180.0f / MathF.PI;
										}

										if (elevationMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											elevation *= 180.0f / MathF.PI;
										}

										Vector3 lightVector = EffectHelpers.GetLightVector(azimuth, elevation);

										return SKImageFilter.CreateDistantLitDiffuse(new SKPoint3(lightVector.X, lightVector.Y, lightVector.Z), color.ToSKColor(), 1.0f, amount, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.DistantSpecularEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 5 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Azimuth", out uint azimuthProp, out GraphicsEffectPropertyMapping azimuthMapping);
										effectInterop.GetNamedPropertyMapping("Elevation", out uint elevationProp, out GraphicsEffectPropertyMapping elevationMapping);
										effectInterop.GetNamedPropertyMapping("SpecularExponent", out uint exponentProp, out _);
										effectInterop.GetNamedPropertyMapping("SpecularAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										float azimuth = (float)(effectInterop.GetProperty(azimuthProp) ?? throw new NullReferenceException("The effect property was null"));
										float elevation = (float)(effectInterop.GetProperty(elevationProp) ?? throw new NullReferenceException("The effect property was null"));
										float exponent = (float)(effectInterop.GetProperty(exponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										if (azimuthMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											azimuth *= 180.0f / MathF.PI;
										}

										if (elevationMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											elevation *= 180.0f / MathF.PI;
										}

										Vector3 lightVector = EffectHelpers.GetLightVector(azimuth, elevation);

										//return SKImageFilter.CreateBlendMode(SKBlendMode.SrcOver, SKImageFilter.CreatePaint(new() { Color = SKColors.Black }), SKImageFilter.CreateDistantLitSpecular(new SKPoint3(lightVector.X, lightVector.Y, lightVector.Z), color.ToSKColor(), 1.0f, amount, exponent, sourceFilter), new(bounds));
										return SKImageFilter.CreateDistantLitSpecular(new SKPoint3(lightVector.X, lightVector.Y, lightVector.Z), color.ToSKColor(), 1.0f, amount, exponent, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.SpotDiffuseEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 6 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("LightPosition", out uint positionProp, out _);
										effectInterop.GetNamedPropertyMapping("LightTarget", out uint targetProp, out _);
										effectInterop.GetNamedPropertyMapping("Focus", out uint focusProp, out _);
										effectInterop.GetNamedPropertyMapping("LimitingConeAngle", out uint angleProp, out GraphicsEffectPropertyMapping angleMapping);
										effectInterop.GetNamedPropertyMapping("DiffuseAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										Vector3 position = (Vector3)(effectInterop.GetProperty(positionProp) ?? throw new NullReferenceException("The effect property was null"));
										Vector3 target = (Vector3)(effectInterop.GetProperty(targetProp) ?? throw new NullReferenceException("The effect property was null"));
										float focus = (float)(effectInterop.GetProperty(focusProp) ?? throw new NullReferenceException("The effect property was null"));
										float angle = (float)(effectInterop.GetProperty(angleProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										if (angleMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											angle *= 180.0f / MathF.PI;
										}

										Vector3 lightTarget = EffectHelpers.CalcLightTargetVector(position, target);
										//Vector2 coneAngle = EffectHelpers.CalcConeAngle(angle); // Uncomment this if we ever needed to switch light effects to pixel shaders

										return SKImageFilter.CreateSpotLitDiffuse(new SKPoint3(position.X, position.Y, position.Z), new SKPoint3(lightTarget.X, lightTarget.Y, lightTarget.Z), focus, angle, color.ToSKColor(), 1.0f, amount, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.SpotSpecularEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 7 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("LightPosition", out uint positionProp, out _);
										effectInterop.GetNamedPropertyMapping("LightTarget", out uint targetProp, out _);
										effectInterop.GetNamedPropertyMapping("Focus", out uint focusProp, out _);
										effectInterop.GetNamedPropertyMapping("LimitingConeAngle", out uint angleProp, out GraphicsEffectPropertyMapping angleMapping);
										effectInterop.GetNamedPropertyMapping("SpecularExponent", out uint exponentProp, out _);
										effectInterop.GetNamedPropertyMapping("SpecularAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										Vector3 position = (Vector3)(effectInterop.GetProperty(positionProp) ?? throw new NullReferenceException("The effect property was null"));
										Vector3 target = (Vector3)(effectInterop.GetProperty(targetProp) ?? throw new NullReferenceException("The effect property was null"));
										float focus = (float)(effectInterop.GetProperty(focusProp) ?? throw new NullReferenceException("The effect property was null"));
										float angle = (float)(effectInterop.GetProperty(angleProp) ?? throw new NullReferenceException("The effect property was null"));
										float exponent = (float)(effectInterop.GetProperty(exponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										if (angleMapping == GraphicsEffectPropertyMapping.RadiansToDegrees)
										{
											angle *= 180.0f / MathF.PI;
										}

										Vector3 lightTarget = EffectHelpers.CalcLightTargetVector(position, target);
										//Vector2 coneAngle = EffectHelpers.CalcConeAngle(angle); // Uncomment this if we ever needed to switch light effects to pixel shaders

										return SKImageFilter.CreateSpotLitSpecular(new SKPoint3(position.X, position.Y, position.Z), new SKPoint3(lightTarget.X, lightTarget.Y, lightTarget.Z), exponent, angle, color.ToSKColor(), 1.0f, amount, focus, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.PointDiffuseEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 3 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("LightPosition", out uint positionProp, out _);
										effectInterop.GetNamedPropertyMapping("DiffuseAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										Vector3 position = (Vector3)(effectInterop.GetProperty(positionProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										return SKImageFilter.CreatePointLitDiffuse(new SKPoint3(position.X, position.Y, position.Z), color.ToSKColor(), 1.0f, amount, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.PointSpecularEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() >= 4 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("LightPosition", out uint positionProp, out _);
										effectInterop.GetNamedPropertyMapping("SpecularExponent", out uint exponentProp, out _);
										effectInterop.GetNamedPropertyMapping("SpecularAmount", out uint amountProp, out _);
										effectInterop.GetNamedPropertyMapping("LightColor", out uint colorProp, out _);

										Vector3 position = (Vector3)(effectInterop.GetProperty(positionProp) ?? throw new NullReferenceException("The effect property was null"));
										float exponent = (float)(effectInterop.GetProperty(exponentProp) ?? throw new NullReferenceException("The effect property was null"));
										float amount = (float)(effectInterop.GetProperty(amountProp) ?? throw new NullReferenceException("The effect property was null"));
										Color color = (Color)(effectInterop.GetProperty(colorProp) ?? throw new NullReferenceException("The effect property was null"));

										return SKImageFilter.CreatePointLitSpecular(new SKPoint3(position.X, position.Y, position.Z), color.ToSKColor(), 1.0f, amount, exponent, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.AlphaMaskEffect: // This is a temporary workaround until we move to Skia 3 so we can implement it properly using pixel shaders
								{
									if (effectInterop.GetSourceCount() == 2 && effectInterop.GetSource(0) is IGraphicsEffectSource source && effectInterop.GetSource(1) is IGraphicsEffectSource mask)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										SKImageFilter? maskFilter = GenerateEffectFilter(mask, bounds);
										if (maskFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										return SKImageFilter.CreateBlendMode(SKBlendMode.SrcIn, maskFilter, sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.SaturationEffect:
								{
									if (effectInterop.GetSourceCount() == 1 && effectInterop.GetPropertyCount() == 1 && effectInterop.GetSource(0) is IGraphicsEffectSource source)
									{
										SKImageFilter? sourceFilter = GenerateEffectFilter(source, bounds);
										if (sourceFilter is null && !_isCurrentInputBackdrop)
										{
											return null;
										}

										_isCurrentInputBackdrop = false;

										effectInterop.GetNamedPropertyMapping("Saturation", out uint saturationProp, out _);

										float saturation = MathF.Min((float)(effectInterop.GetProperty(saturationProp) ?? throw new NullReferenceException("The effect property was null")), 2);

										return SKImageFilter.CreateColorFilter(
											SKColorFilter.CreateColorMatrix(
												new float[] // Saturation Matrix
												{
													0.2126f + 0.7874f * saturation, 0.7152f - 0.7152f * saturation, 0.0722f - 0.0722f * saturation, 0.0f, 0.0f,
													0.2126f - 0.2126f * saturation, 0.7152f + 0.2848f * saturation, 0.0722f - 0.0722f * saturation, 0.0f, 0.0f,
													0.2126f - 0.2126f * saturation, 0.7152f - 0.7152f * saturation, 0.0722f + 0.9278f * saturation, 0.0f, 0.0f,
													0.0f,                           0.0f,                           0.0f,                           1.0f, 0.0f
												}),
											sourceFilter, new(bounds));
									}

									return null;
								}
							case EffectType.WhiteNoiseEffect:
								{
									if (effectInterop.GetPropertyCount() == 2)
									{
										effectInterop.GetNamedPropertyMapping("Frequency", out uint frequencyProp, out _);
										effectInterop.GetNamedPropertyMapping("Offset", out uint offsetProp, out _);

										Vector2 frequency = (Vector2)(effectInterop.GetProperty(frequencyProp) ?? throw new NullReferenceException("The effect property was null"));
										Vector2 offset = (Vector2)(effectInterop.GetProperty(offsetProp) ?? throw new NullReferenceException("The effect property was null"));

										string shader = $@"
											uniform half2 frequency;
											uniform half2 offset;

											half Hash(half2 p)
											{{
												return fract(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x))));
											}}


											half4 main(float2 coords) 
											{{
												float2 coord = coords * 0.81 * frequency + offset;
												float2 px00 = floor(coord - 0.5) + 0.5;
												float2 px11 = px00 + 1;
												float2 px10 = float2(px11.x, px00.y);
												float2 px01 = float2(px00.x, px11.y);
												float2 factor = coord - px00;
												float sample00 = Hash(px00);
												float sample10 = Hash(px10);
												float sample01 = Hash(px01);
												float sample11 = Hash(px11);
												float result = mix(mix(sample00, sample10, factor.x), mix(sample01, sample11, factor.x), factor.y);

												return half4(result.xxx, 1);
											}}
										";

										SKRuntimeEffect runtimeEffect = SKRuntimeEffect.Create(shader, out string errors);
										if (errors is not null)
										{
											return null;
										}

										SKRuntimeEffectUniforms uniforms = new(runtimeEffect)
										{
											{ "frequency", new float[2] { frequency.X, frequency.Y } },
											{ "offset", new float[2] { offset.X, offset.Y } }
										};

										return SKImageFilter.CreatePaint(new SKPaint() { Shader = runtimeEffect.ToShader(false, uniforms), IsAntialias = true, FilterQuality = SKFilterQuality.High }, new(bounds));
									}

									return null;
								}
							case EffectType.Unsupported:
							default:
								return null;
						}
					}
				default:
					return null;
			}
		}

		internal override void UpdatePaint(SKPaint paint, SKRect bounds)
		{
			if (_currentBounds != bounds || _filter is null)
			{
				_isCurrentInputBackdrop = false;
				HasBackdropBrushInput = false;
				_filter = GenerateEffectFilter(_effect, bounds) ?? throw new NotSupportedException($"Unsupported effect description.\r\nEffect name: {_effect.Name}");
				_currentBounds = bounds;
			}

			paint.Shader = null;
			paint.ImageFilter = _filter;
			paint.FilterQuality = SKFilterQuality.High;
		}

		private protected override void DisposeInternal()
		{
			_filter?.Dispose();
		}
	}
}