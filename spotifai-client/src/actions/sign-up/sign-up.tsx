"use server";

import { FormState, SignupFormSchema } from "@/actions/sign-up/sign-up-scheme";

export async function signup(state: FormState, formData: FormData) {
  console.log({
    formData,
    state
  });

  const validatedFields = SignupFormSchema.safeParse({
    email: formData.get("email"),
    password: formData.get("password")
  });

  if (!validatedFields.success) {
    return {
      apiError: null,
      errors: validatedFields.error.flatten().fieldErrors,
      success: false
    };
  }

  const backendRootUrl = process.env.BACKEND_ROOT_URL!;

  const response = await fetch(`${backendRootUrl}/users/register`, {
    method: "POST",
    body: JSON.stringify({
      email: validatedFields.data.email,
      password: validatedFields.data.password
    }),
    headers: {
      "Content-Type": "application/json"
    }
  });

  const responseBody = await response.json();

  if (!response.ok) {
    return {
      apiError: [responseBody] as string[],
      errors: {
        email: [],
        password: []
      },
      success: false
    };
  }

  return {
    success: true
  };
}
