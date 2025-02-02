import { auth } from "../../../auth";

export default async function Page() {
  const session = await auth();
  if (!session) {
  }
  console.log(session);

  return <div>Dashboard</div>;
}
