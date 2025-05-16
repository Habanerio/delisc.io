import { LinkResults } from '@/components/LinkResults';

export default function Home({
   searchParams,
}: {
   searchParams?: { [key: string]: string | string[] | undefined };
}) {
   return <LinkResults searchParams={searchParams} />;
}
